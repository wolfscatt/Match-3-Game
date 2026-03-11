# Match-3 — Portfolio Project

A production-quality Match-3 puzzle game built with Unity 6.

## Architecture
Clean Architecture (Domain / Application / Infrastructure / Presentation)
- **DI:** VContainer
- **Pattern:** State Machine + EventBus + Object Pool
- **Animations:** DOTween
- **Tests:** EditMode unit tests (NUnit)

## Project Structure
Assets/Scripts/
├── Domain/          # Pure C# — zero Unity dependency
│   ├── Board/       # BoardModel, CellType
│   ├── Common/      # GoalData, shared value objects
│   ├── Events/      # EventBus, GameEvents
│   ├── Match/       # IMatchStrategy, MatchGroup, MatchResult
│   ├── Rules/       # BoardGenerator, GravitySystem, ShuffleService, SpecialTileRule
│   └── Tiles/       # TileModel, TileColor, TileSpecialType
├── Application/     # Use cases, state machine, services
│   ├── DTOs/        # SwapResult
│   ├── Services/    # GoalService, ScoreService, MoveService, SceneService
│   ├── StateMachine/# GameStateMachine + States
│   └── UseCases/    # SwapTilesUseCase, InitializeBoardUseCase
├── Infrastructure/  # Unity implementations
│   ├── Audio/       # AudioService, AudioConfig
│   ├── Config/      # LevelConfig, TileViewConfig
│   ├── DI/          # GameInstaller (VContainer LifetimeScope)
│   ├── Factories/   # TileFactory, TileViewConfig
│   ├── Input/       # InputHandler (Unity Input System)
│   └── Pool/        # ObjectPool<T>
└── Presentation/    # Views, animations, UI
    ├── Animation/   # AnimationSequencer
    ├── Board/       # TileView, BoardView, BoardController
    ├── HUD/         # HUDView, GoalEntryUI, UIManager
    ├── Popups/      # BasePopup, WinPopup, LosePopup, PausePopup
    └── VFX/         # VFXService


## Features
### ✅ Implemented
- [x] Domain layer — pure C#, zero Unity dependency
- [x] BoardModel with bounds validation and cell type support
- [x] Match detection — horizontal, vertical, L/T shape, 4+, 5+
- [x] Special tile rules — Bomb (3x3), RocketH, RocketV, ColorBomb
- [x] Board generation — no-match initial fill
- [x] Gravity system — tile fall + refill
- [x] Shuffle service — deadlock detection + Fisher-Yates
- [x] Game state machine — Idle / Swap / Match / Fall / Win / Lose
- [x] SwapTilesUseCase — adjacency validation + match check
- [x] InitializeBoardUseCase — board generate + service init
- [x] Goal system — track X of color
- [x] Score system — base score + combo multiplier
- [x] Move limit system
- [x] EventBus — type-safe, loose coupling
- [x] VContainer DI — GameInstaller with full dependency graph
- [x] ObjectPool<T> — generic MonoBehaviour pooling
- [x] InputHandler — Unity Input System (mouse + touch)
- [x] AudioService — BGM loop + SFX oneshot
- [x] LevelConfig — ScriptableObject driven level data
- [x] TileFactory — pool-backed tile view creation
- [x] TileView — DOTween animations (swap, match, fall, spawn, special upgrade)
- [x] BoardView — grid layout, world↔grid position conversion
- [x] BoardController — input → usecase → animation game loop
- [x] AnimationSequencer — chained animation helper
- [x] VFXService — EventBus-driven particle effects
- [x] HUDView — move counter, animated score, goal tracker
- [x] GoalEntryUI — per-goal progress with completion animation
- [x] UIManager — coordinates all UI elements
- [x] BasePopup — DOTween show/hide base class
- [x] WinPopup — level complete screen with star animation
- [x] LosePopup — out of moves screen
- [x] PausePopup — pause menu with audio sliders
- [x] SceneService — scene transition management

### 📋 Planned
- [ ] EditMode unit tests
- [ ] Level ScriptableObjects (5 sample levels)

## Setup
1. Unity 6 (6000.x LTS)
2. Install VContainer via Package Manager
3. Install DOTween via Package Manager
4. Open `Assets/Scenes/Gameplay.unity`

## Development Progress
| Phase | Status | Description |
|-------|--------|-------------|
| Phase 1 | ✅ Done | Domain layer foundation |
| Phase 2 | ✅ Done | Domain rules + Application layer |
| Phase 3 | ✅ Done | Infrastructure + VContainer DI |
| Phase 4 | ✅ Done | Presentation + Unity layer |
| Phase 5 | ✅ Done | UI + VFX |
| Phase 6 | 🔄 WIP  | Unit tests |
