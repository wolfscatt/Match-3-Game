# Match-3 — Portfolio Project

A production-quality Match-3 puzzle game built with Unity 6.

## Architecture
Clean Architecture (Domain / Application / Infrastructure / Presentation)
- **DI:** VContainer
- **Pattern:** State Machine + EventBus + Object Pool
- **Tests:** EditMode unit tests (NUnit)

## Features
- [ ] Core match detection (3+, L/T shape)
- [ ] Special tiles (Bomb, Rocket H/V, Color Bomb)
- [ ] Goal system (collect X of color)
- [ ] Move limit
- [ ] Blocked cells
- [ ] Object pooling
- [ ] Level configs via ScriptableObject

## Project Structure
Assets/Scripts/
├── Domain/          # Pure C# — zero Unity dependency
├── Application/     # Use cases, state machine, services
├── Infrastructure/  # Unity implementations (audio, input, pool)
└── Presentation/    # Views, animations, UI

## Setup
1. Unity 6 (6000.x LTS)
2. Install VContainer via Package Manager
3. Open `Assets/Scenes/GameScene.unity`