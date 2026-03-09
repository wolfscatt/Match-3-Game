# Match-3 — Portfolio Project

A production-quality Match-3 puzzle game built with Unity 6.

## Architecture
Clean Architecture (Domain / Application / Infrastructure / Presentation)
- **DI:** VContainer
- **Pattern:** State Machine + EventBus + Object Pool
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
│   ├── Services/    # GoalService, ScoreService, MoveService
│   ├── StateMachine/# GameStateMachine + States
│   └── UseCases/    # SwapTilesUseCase
├── Infrastructure/  # Unity implementations (audio, input, pool, config)
└── Presentation/    # Views, animations, UI

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
- [x] Goal system — track X of color
- [x] Score system — base score + combo multiplier
- [x] Move limit system
- [x] EventBus — type-safe, loose coupling

### 🔄 In Progress
- [ ] VContainer DI setup (GameInstaller)
- [ ] Infrastructure layer (Config, Pool, Input, Audio)

### 📋 Planned
- [ ] Presentation layer (TileView, BoardView, animations)
- [ ] UI — HUD, Win/Lose popups
- [ ] VFX — particle effects
- [ ] EditMode unit tests
- [ ] Level ScriptableObjects (5 sample levels)

## Setup
1. Unity 6 (6000.x LTS)
2. Install VContainer via Package Manager
3. Open `Assets/Scenes/Gameplay.unity`

## Development Progress
| Phase | Status | Description |
|-------|--------|-------------|
| Phase 1 | ✅ Done | Domain layer foundation |
| Phase 2 | ✅ Done | Domain rules + Application layer |
| Phase 3 | 🔄 WIP  | Infrastructure + VContainer DI |
| Phase 4 | 📋 Todo | Presentation + Unity layer |
| Phase 5 | 📋 Todo | UI + VFX |
| Phase 6 | 📋 Todo | Unit tests |
