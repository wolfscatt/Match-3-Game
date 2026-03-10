using Match3Game.Application.Services;
using Match3Game.Application.StateMachine;
using Match3Game.Application.StateMachine.States;
using Match3Game.Application.UseCases;
using Match3Game.Domain.Board;
using Match3Game.Domain.Events;
using Match3Game.Domain.Match;
using Match3Game.Domain.Rules;
using Match3Game.Infrastructure.Audio;
using Match3Game.Infrastructure.Config;
using Match3Game.Infrastructure.Factories;
using Match3Game.Infrastructure.Input;
using Match3Game.Infrastructure.Pool;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Match3Game.Infrastructure.DI
{
    /// <summary>
    /// VContainer LifetimeScope -- tüm bağımlılıkları tek noktada bağlar.
    /// Yeni bir sistem eklendiğinde sadece burası güncellenir.
    /// </summary>
    public class GameInstaller : LifetimeScope
    {
        [Header("Config")]
        [SerializeField] private LevelConfig _levelConfig;
        [SerializeField] private TileViewConfig _tileViewConfig;
        [SerializeField] private AudioConfig _audioConfig;

        [Header("Prefabs")]
        [SerializeField] private TileView _tilePrefab;

        [Header("Scene References")]
        [SerializeField] private Transform _boardRoot;
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterCore(builder);
            RegisterDomain(builder);
            RegisterApplication(builder);
            RegisterInfrastructure(builder);
            RegisterPresentation(builder);
        }

        // ── Core ────────────────────────────────────────────────────

        private void RegisterCore(IContainerBuilder builder)
        {
            // EventBus -- singleton, tüm sistemler paylaşır.
            builder.Register<EventBus>(Lifetime.Singleton)
                .As<IEventBus>();
            
            // Random -- seed'li olabilir (test için)
            builder.RegisterInstance(new System.Random());
        }

        // ── Domain ──────────────────────────────────────────────────

        private void RegisterDomain(IContainerBuilder builder)
        {
            // BoardModel -- level config'ten üretilir
            builder.Register<BoardModel>(resolver =>
            {
                var cellTypes = _levelConfig.BuildCellTypeGrid();
                return new BoardModel(_levelConfig.rows, _levelConfig.cols, cellTypes);
            }, Lifetime.Singleton);

            // Match Strategy -- StandardMatchStrategy varsayılan
            // Yeni strateji eklemek için sadece bu satırı değiştir (OCP)
            builder.Register<IMatchStrategy, StandardMatchStrategy>(Lifetime.Singleton);

            // Domain Rules
            builder.Register<BoardGenerator>(Lifetime.Singleton);
            builder.Register<GravitySystem>(Lifetime.Singleton);
            builder.Register<ShuffleService>(Lifetime.Singleton);
            builder.Register<SpecialTileRule>(Lifetime.Singleton);
        }

        // ── Application ─────────────────────────────────────────────

        private void RegisterApplication(IContainerBuilder builder)
        {
            // Services
            builder.Register<GoalService>(Lifetime.Singleton);
            builder.Register<ScoreService>(Lifetime.Singleton);
            builder.Register<MoveService>(Lifetime.Singleton);

            // Use Cases
            builder.Register<SwapTilesUseCase>(Lifetime.Singleton);

            // State Machine
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.Register<IdleState>(Lifetime.Singleton);
            builder.Register<MatchState>(Lifetime.Singleton);
            builder.Register<FallState>(Lifetime.Singleton);
            builder.Register<SwapState>(Lifetime.Singleton);
            builder.Register<WinState>(Lifetime.Singleton);
            builder.Register<LoseState>(Lifetime.Singleton);
        }

        // ── Infrastructure ───────────────────────────────────────────

        private void RegisterInfrastructure(IContainerBuilder builder)
        {
            // Config'leri instance olarak kaydet
            builder.RegisterInstance(_levelConfig);
            builder.RegisterInstance(_tileViewConfig);
            builder.RegisterInstance(_audioConfig);

            // Input
            builder.RegisterInstance(_inputHandler);

            // Audio
            builder.Register<AudioService>(resolver =>
                new AudioService(_bgmSource, _sfxSource, _audioConfig),
                Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();

            // Object Pool
            builder.Register<ObjectPool<TileView>>(resolver =>
                new ObjectPool<TileView>(
                    _tilePrefab,
                    _boardRoot,
                    initialSize: _levelConfig.rows * _levelConfig.cols,
                    maxSize: _levelConfig.rows * _levelConfig.cols + 20
                ), Lifetime.Singleton);

            // Factory
            builder.Register<TileFactory>(Lifetime.Singleton);
        }

        // ── Presentation ─────────────────────────────────────────────

        private void RegisterPresentation(IContainerBuilder builder)
        {
            // BoardController -- IStartable ile oyun başlar
            builder.RegisterComponentInHierarchy<BoardController>();
        }

    }

}
