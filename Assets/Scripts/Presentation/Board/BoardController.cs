using System.Collections;
using Match3Game.Application.StateMachine;
using Match3Game.Application.StateMachine.States;
using Match3Game.Application.UseCases;
using Match3Game.Domain.Board;
using Match3Game.Domain.Events;
using Match3Game.Domain.Match;
using Match3Game.Domain.Rules;
using Match3Game.Infrastructure.Config;
using Match3Game.Infrastructure.Factories;
using Match3Game.Infrastructure.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Match3Game.Presentation.Board
{
    /// <summary>
    /// Oyunun ana koordinatörü.
    /// Input -> UseCase -> View animasyonu -> döngü.
    /// IStartable: VContainer oyun başında otomatik çağırır.
    /// </summary>
    public class BoardController : MonoBehaviour, IStartable
    {
        // ── Injected ─────────────────────────────────────────────────
        private BoardModel _boardModel;
        private BoardView _boardView;
        private GameStateMachine _stateMachine;
        private SwapTilesUseCase _swapUseCase;
        private InitializeBoardUseCase _initUseCase;
        private GravitySystem _gravitySystem;
        private ShuffleService _shuffleService;
        private IEventBus _eventBus;
        private InputHandler _inputHandler;
        private LevelConfig _levelConfig;
        private TileViewConfig _tileViewConfig;
        private SwapState _swapState;

        [Inject]
        public void Construct(
            BoardModel boardModel,
            BoardView boardView,
            GameStateMachine stateMachine,
            SwapTilesUseCase swapUseCase,
            InitializeBoardUseCase initUseCase,
            GravitySystem gravitySystem,
            ShuffleService shuffleService,
            IEventBus eventBus,
            InputHandler inputHandler,
            LevelConfig levelConfig,
            TileViewConfig tileViewConfig,
            SwapState swapState)
        {
            _boardModel = boardModel;
            _boardView = boardView;
            _stateMachine = stateMachine;
            _swapUseCase = swapUseCase;
            _initUseCase = initUseCase;
            _gravitySystem = gravitySystem;
            _shuffleService = shuffleService;
            _eventBus = eventBus;
            _inputHandler = inputHandler;
            _levelConfig = levelConfig;
            _tileViewConfig = tileViewConfig;
            _swapState = swapState;
        }

        // ── Lifecycle ────────────────────────────────────────────────

        public void Start()
        {
            RegisterStates();
            SubscribeEvents();
            SubscribeInput();

            _initUseCase.Execute();
            _stateMachine.ChangeState<IdleState>();
        }

        private void Update() => _stateMachine.Tick();

        private void OnDestroy() => UnsubscribeAll();

        // ── State Machine Setup ──────────────────────────────────────

        private void RegisterStates()
        {
            // VContainer inject edilmiş state'leri kayder
            var resolver = GetComponent<LifetimeScope>();

            _stateMachine.RegisterState(resolver.Container.Resolve<IdleState>());
            _stateMachine.RegisterState(resolver.Container.Resolve<MatchState>());
            _stateMachine.RegisterState(resolver.Container.Resolve<SwapState>());
            _stateMachine.RegisterState(resolver.Container.Resolve<FallState>());
            _stateMachine.RegisterState(resolver.Container.Resolve<WinState>());
            _stateMachine.RegisterState(resolver.Container.Resolve<LoseState>());
        }

        // ── Input ────────────────────────────────────────────────────

        private TileView _selectedView;

        private void SubscribeInput()
        {
            _inputHandler.OnTilePressed += HandlePress;
            _inputHandler.OnTileReleased += HandleRelease;
        }

        private void HandlePress(Vector2 worldPos)
        {
            if(!_stateMachine.IsIn<IdleState>()) return;
            if(!_boardView.TryGetGridPosition(worldPos, out int row, out int col)) return;

            var view = _boardView.GetView(row, col);
            if(view == null) return;

            _selectedView = view;
            _selectedView.AnimateSelect();
        }

        private void HandleRelease(Vector2 worldPos)
        {
            if(_selectedView == null) return;
            if(!_boardView.TryGetGridPosition(worldPos, out int row, out int col)) return;

            var targetView = _boardView.GetView(row, col);
            if(targetView == null || targetView == _selectedView)
            {
                _selectedView.AnimateDeselect();
                _selectedView = null;
                return;
            }

            // Swap dene
            _selectedView.AnimateDeselect();

            int r1 = _selectedView.Model.Row, c1 = _selectedView.Model.Col;
            int r2 = row, c2 = col;
            _selectedView = null;

            TrySwap(r1, c1, r2, c2);
        }

        // ── Swap Flow ────────────────────────────────────────────────

        private void TrySwap(int r1, int c1, int r2, int c2)
        {
            _inputHandler.Disable();
            _stateMachine.ChangeState<SwapState>();

            // Önce view animasyonu göster
            _boardView.AnimateSwap(r1, c1, r2, c2, () =>
            {
                var result = _swapUseCase.Execute(r1, c1, r2, c2);

                if (result.IsValid)
                {
                    // Match var -- temizle -> gravity -> tekrar kontrol
                    StartCoroutine(ProcessMatchCycle());
                }
                else
                {
                    // Geçersiz -- geri al
                    _boardView.AnimateInvalidSwap(r1, c1, r2, c2, () =>
                    {
                        _stateMachine.ChangeState<IdleState>();
                        _inputHandler.Enable();
                    });
                }
            });
        }

        // ── Match → Gravity Döngüsü ──────────────────────────────────

        private IEnumerator ProcessMatchCycle()
        {
            _stateMachine.ChangeState<MatchState>();

            bool continueLoop = true;

            while (continueLoop)
            {
                // 1. Match bul
                var matchResult = new StandardMatchStrategy()
                    .FindMatches(_boardModel);

                if(!matchResult.HasMatch) break;

                // 2. Match animasyonu
                bool matchDone = false;
                _boardView.AnimateMatches(matchResult.Groups, () => matchDone = true);
                yield return new WaitUntil(() => matchDone);

                // 3. Domain'de tile'ları temizle
                foreach(var group in matchResult.Groups)
                    foreach(var tile in group.Tiles)
                        _boardModel.ClearTile(tile.Row, tile.Col);

                _eventBus.Publish(new MatchFoundEvent(matchResult));

                // 4. Gravity uygula
                _stateMachine.ChangeState<FallState>();

                var moves = _gravitySystem.ApplyGravity(_boardModel);
                bool fallDone = false;
                _boardView.AnimateFalls(moves, () => fallDone = true);
                yield return new WaitUntil(() => fallDone);

                // 5. Refill
                var spawns = _gravitySystem.FillEmpty(
                    _boardModel,
                    _levelConfig.allowedColors,
                    new System.Random());

                _boardView.AnimateSpawns(spawns, _tileViewConfig);
                yield return new WaitForSeconds(0.3f); // Spawn Animasyonu

                _eventBus.Publish(new TilesFellEvent());

                // 6. Cascade -- tekrar match var mı?
                var cascadeCheck = new StandardMatchStrategy()
                    .FindMatches(_boardModel);

                continueLoop = cascadeCheck.HasMatch;
            }

            // 7. Deadlock kontrolü
            if (!_shuffleService.HasValidMove(_boardModel))
            {
                _eventBus.Publish(new BoardDeadlockedEvent());
                _shuffleService.Shuffle(_boardModel);
                // View'ı yenile
                yield return StartCoroutine(RefreshAllViews());
            }

            _stateMachine.ChangeState<IdleState>();
            _inputHandler.Enable();
        }

        private IEnumerator RefreshAllViews()
        {
            yield return new WaitForSeconds(0.2f);
            _boardView.SpawnAllTiles(_boardModel, _tileViewConfig);
        }

        // ── Event Subscriptions ──────────────────────────────────────

        private void SubscribeEvents()
        {
            _eventBus.Subscribe<LevelWonEvent>(OnLevelWon);
            _eventBus.Subscribe<LevelLostEvent>(OnLevelLost);
            _eventBus.Subscribe<BoardInitializedEvent>(OnBoardInitialized);
        }

        private void OnBoardInitialized(BoardInitializedEvent e)
        {
            _boardView.Initialize(e.Rows, e.Cols);
            _boardView.SpawnAllTiles(_boardModel, _tileViewConfig);
        }

        private void OnLevelWon(LevelWonEvent _)
        {
            _inputHandler.Disable();
            _stateMachine.ChangeState<WinState>();
        }

        private void OnLevelLost(LevelLostEvent _)
        {
            _inputHandler.Disable();
            _stateMachine.ChangeState<LoseState>();
        }

        private void UnsubscribeAll()
        {
            _inputHandler.OnTilePressed -= HandlePress;
            _inputHandler.OnTileReleased -= HandleRelease;
            _eventBus.Unsubscribe<LevelWonEvent>(OnLevelWon);
            _eventBus.Unsubscribe<LevelLostEvent>(OnLevelLost);
            _eventBus.Unsubscribe<BoardInitializedEvent>(OnBoardInitialized);
        }

    }
}

