# Shape Keeper 🛡️
카드로 도형 유닛을 소환하고 합성하여 행성을 지키는 로그라이크 디펜스 게임, Unity 2D 제작

## 👤 개발 정보

* **개발자**: 권순호
* **장르**: 로그라이크 디펜스 / 자동 전투
* **개발 기간**: 2025.01 ~ 2025.03
* **개발 인원**: 1인 개발
* **담당 역할**: 1인 개발로 인한 전체 담당

## ✨ Features

* 카드 기반 유닛 소환 — 덱에서 카드를 뽑아 필드에 도형 유닛 배치
* 자유로운 타워 배치 / 이동(Shift) / 회수(Recall)
* 동일 유닛 합성(Combine)으로 상위 등급 진화
* 스펠 시스템 — 즉발형 스킬 발동 및 강화
* **A\* 알고리즘 기반 Tilemap 경로 탐색** — 유닛 설치로 막힌 길을 실시간으로 재계산
* 4가지 테마 행성(Green / Blue / Gray / Ancient) 선택형 스테이지 진행 구조
* 6가지 필드 격자 형태 (1 / 2가로 / 2세로 / 4 / 6가로 / 6세로)
* 적 스폰 & 라운드 진행 (최대 100 라운드)
* 업적 / 미션 / 출석 체크 / 스태미나 / 상점 / 상자 구매 / 에너지 구매
* 게임 속도 조절 · 난이도 선택 · 일시정지

## 🛠 Tech Stack

### 엔진 / 언어

* **Engine**: Unity 6 (6000.0.63f1) + URP 17.0.4 — Unity 6 신규 셰이더 시스템을 모바일 환경에서 검증
* **2D / Tilemap**: `com.unity.feature.2d` 2.0.1 — Tilemap 기반 스테이지 구성 및 길찾기 그리드의 기반
* **Input**: Unity Input System 1.16.0 — 신규 입력 시스템 적용
* **Tweening**: DOTween (Demigiant) — UI · 이펙트 트위닝, 콜백 체이닝으로 코루틴 대비 흐름 단순화
* **Language**: C#
* **Target Platform**: Android (세로 모드, v1.3.1 / Bundle 12 출시)

### 데이터

* **Database**: SQLite (`Mono.Data.Sqlite`) — `StreamingAssets/ShapeKeeperDB.db`에 업적 · 미션 · 유닛 · 유저 진행도 로컬 영속화
* **중앙 데이터 허브** — `Game_State_Data` 정적 클래스로 씬 · DB 상태 · 난이도 · 게임 속도 · 라운드 · 스태미나를 일원화 (전역 접근 1지점)
* **도메인 데이터 캐시** — `Game_Data` 정적 영역에 유닛 사전(`unit_dic`) · 유닛 카운터(`unit_counter`) · 합성 함수(`CombineFunction`)를 로드해 런타임 조회를 O(1)에 처리

### 아키텍처 패턴

* **싱글톤 2계층** — 영속(`Singleton<T>` + `DontDestroyOnLoad`, ex. `ModifyDB`) / 씬 한정(`Scene_Singleton<T>`, ex. `Bullet_Pool`)으로 매니저 수명을 명시적으로 분리
* **Observer 분배 매니저** — `Achievement_Observer` · `Stamina_Observer` · `Day_Change_Observer` · `Combine_Observer` · `Spell_Recognition_Observer`가 도메인 이벤트 발생 시 UI · 시스템에 변경을 전파하는 중앙 진입점 역할
* **상태 기반 FSM** — 유닛 행동(공격 / 대기 / 회수 / 변환)을 상태로 분리하여 분기 폭주 방지
* **Pipeline 분리** — 카드 인터랙션을 단방향 흐름으로 모듈화 `Card_To_Summon` → `TowerInstaller` → `Summon` → (`Shift` / `Recall` / `Special_Unit_Type_Changer`)

### 최적화

* **Queue 기반 오브젝트 풀링** — 8종 전용 풀 (Bullet · Enemy · Effect · Card · Combine · Area · LockOn · Ora). Enemy는 `ori` / `shield` / `bust` 3종으로 분리, Effect / Ora는 `Dictionary<OraType, Queue<>>` 구조로 타입별 분리
* **풀 자동 증량** — `Bullet_Pool`은 100개로 초기화하되, `Get()` 시점에 부족하면 즉시 `Create()`로 런타임 무중단 확장 (전투 절정에도 풀 고갈 없음)
* **A\* Pathfinding** (Tilemap, 8방향, 대각 비용 14·직선 10) — 평균 탐색 0.04초. 유저 타워 설치로 경로가 막히면 `Re_Path_Finding`이 가장 가까운 도달 가능 셀부터 즉시 재탐색
* **SQL 쓰기 큐 + 코루틴 비동기** — `ModifyDB`가 `query_queue` / `db_queue`로 변경 쿼리를 큐잉, `StartCoroutine(Modify_Active())`로 매 프레임 1건씩 처리하여 메인 스레드 블로킹 회피
* **`DB_STATE` 플래그** — 쓰기 큐가 비고 처리가 끝난 시점에만 `Usable`로 전환, 읽기-쓰기 충돌 방지

## 📱 다운로드

* **[Google Play](https://play.google.com/store/apps/details?id=com.SunhosWorld.ShapeKeeper&hl=ko&pli=1)**

## 🎮 게임 개요

플레이어는 카드 덱에서 도형 유닛(원/사각형/삼각형 등)을 뽑아 필드에 배치하고, 같은 유닛을 합성(Combine)하여 더 강한 유닛으로 진화시키며 행성을 향해 진격하는 적들을 막아내는 카드 + 타워 디펜스 게임입니다.

### 🎯 기획 의도

* 간편한 조작과 배치를 통해 다양한 전술 플레이가 가능한 자동 전투형 디펜스 구현
* 유닛 배치로 적의 이동 경로 자체를 제한할 수 있는, "배치"가 곧 전략이 되는 디펜스 구현

### 주요 시스템

* **유닛** — 카드에서 소환, 배치, 이동(Shift), 회수(Recall), 특수 유닛 변환, 합성(Combine)
* **전투** — 자동 공격, 타겟 인식(Lock-On), 총알 풀링, 라운드 진행, 미션 달성 판정
* **합성(Combine)** — Combine Board / Field / Book / Function Creater 로 구성된 깊이 있는 합성 루프
* **스펠** — 생성(Generate) → 인식(Recognition Observer) → 발동(Active) → 강화(Upgrade) 파이프라인
* **스테이지** — `Plannet` 씬에서 4종 행성(Green / Blue / Gray / Ancient) 선택, 6가지 필드 격자로 다양한 레이아웃 제공
* **콘텐츠**
   * 업적(Achievement)
   * 일일·주간 미션(Mission / Quest)
   * 출석 체크(Attendance)
   * 상점(Shop) / 상자 구매(Chest) / 스태미나(Energy) 구매
* **편의 기능** — 게임 속도 조절(`game_speed`), 난이도 선택, 일시정지, 닉네임/UI 커스터마이징

## 🔧 핵심 시스템

### 제네릭 싱글톤 & 전역 상태 허브

`Assets/Resources/02.Script/Data/Singleton.cs`에 정의된 `Singleton<T> : MonoBehaviour` 패턴을 통해 `ConnectDB`, `ModifyDB` 등 매니저 클래스가 씬 전환 후에도 유지됩니다(`DontDestroyOnLoad`).

전역 게임 상태는 `Data/DataStorage.cs`의 정적 클래스 `Game_State_Data`에 모여 있어, 현재 씬, DB 상태, 난이도, 게임 속도, 라운드, 스태미나 등을 한 곳에서 추적합니다.

### SQLite 데이터베이스

`Mono.Data.Sqlite`를 사용해 `Assets/StreamingAssets/ShapeKeeperDB.db`에 접근합니다. `ConnectDB`가 모든 읽기·쓰기를 담당하고, `ModifyDB`가 변경 트랜잭션을 처리합니다. 업적·미션·유닛 데이터를 DB에 영속화하여 세션 간 진행 상태를 보존합니다.

### A\* 기반 Tilemap 경로 탐색

`EnemyPathChecker`가 Tilemap의 모든 셀을 노드화한 뒤 A\* 알고리즘으로 적의 이동 경로를 계산합니다.

* **행성별 통행 규칙** — `Game_State_Data.Now_plannet`에 따라 통과 가능 타일이 달라짐 (Green: 54·55·79 / Blue: 69·71·79 / Gray: 75·79·801 / Ancient: 70·72·79)
* **8방향 탐색** — 직선 이동 비용 10, 대각선 14, 대각 이동 시 좌우 통행 가능 여부까지 검사
* **실시간 재탐색** — 유저가 타워를 설치해 경로가 막히면 `Re_Path_Finding`이 현재 위치 기준으로 가장 가까운 도달 가능 셀을 찾아 경로 갱신
* **평균 탐색 시간 ≈ 0.04초**

### 오브젝트 풀링

런타임에 빈번하게 생성/파괴되는 객체를 8종의 전용 풀(`Bullet_Pool`, `Enemy_Pool`, `Effect_Pooling`, `Unit_Card_Pool`, `Combine_Pool`, `Area_Pool`, `Lock_On_Pool`, `Ora_Pool`)로 분리하여 GC 부하와 프레임 드랍을 최소화했습니다.

### 카드 → 소환 파이프라인

카드 인터랙션을 단일 흐름으로 통합했습니다.

```
Card_To_Summon  →  TowerInstaller  →  Summon
                                          ├─ Shift          (위치 이동)
                                          ├─ Recall         (회수)
                                          └─ Special_Unit_Type_Changer
```

`FIeld_Unit_Click_Setter`가 필드 클릭 입력을 받아 위 흐름을 분기시키고, `Attack`, `Bullet_Mover`가 자동 전투를 담당합니다.

### 옵저버 패턴

`Observer/` 폴더의 `Achievement_Observer`, `Day_Change_Observer`, `Stamina_Observer`, 그리고 `Spell_Recognition_Observer`, `Combine_Observer` 등이 게임 이벤트를 구독해 UI/로직 결합도를 낮춥니다.

### 합성(Combine) 시스템

`Combine_Board`(드래그 보드), `Combine_Field`(전장 합성), `Combine_Book`(도감), `Combine_Function_Creater`(레시피 생성), `Combine_Observer`(결과 통지)로 구성되어 머지 메커닉의 상호작용을 모듈화했습니다.

## 🐞 문제 해결 사례

### 1. 미로에서 유닛이 엉뚱한 경로로 이동하거나 길이 막힘

**원인 분석**
* 실시간 타일맵 변경(타워 설치)을 기존 길찾기 로직이 반영하지 못함

**해결 방안**
* A\* 알고리즘으로 Tilemap 기반 경로 탐색을 새로 구현
* 유저가 벽(타워)을 추가하는 순간 즉시 경로 재탐색
* 도달 가능 여부 + 거리 기준으로 다음 경로를 결정

**결과**
* 경로 탐색 평균 **0.04초**
* 실시간 반응형 경로 시스템 완성

### 2. 보드에 유닛 배치 시 충돌·겹침 문제

**원인 분석**
* 보드 셀과 유닛 위치 연동이 제대로 이루어지지 않아 유닛이 셀 경계 위에 어긋나 배치됨

**해결 방안**
* 기존 "바닥을 변경한 뒤 유닛을 배치"하던 흐름을 "유닛을 중심으로 바닥을 재구성"하는 방식으로 역전
* 바닥 설치를 칸 단위로 분리하지 않고, 6개의 Pivot 기준으로 통합 설치하도록 변경

**결과**
* 유닛 겹침 / 비정상 위치 배치 현상 제거
* 시각적 일관성 및 배치 정확도 향상

## 📜 도전 & 학습한 점

* **객체 풀링** 적용으로 빈번한 생성/파괴를 제거하고 GC 부담 감소
* **업그레이드 · 강화 · 조합**을 묶어 깊이 있는 성장 루프 설계
* **방치형 리소스 처리** — 출석 / 스태미나 / 일·주 미션의 시간 기반 갱신
* **자동화된 전투 시스템** — 입력 최소화 + 배치만으로 전략이 갈리는 흐름 구성
* **웨이브 난이도 밸런싱** — 적 HP·스폰 주기·보상 곡선 조정으로 100라운드 진행감 확보
* **UI 직관성** — 유닛 상태, 자원, 조합 가능 여부를 한눈에 보여주는 패널 설계
* **Google Play Store 출시** 경험 — 빌드·서명·심사·업데이트(현재 v1.3.1, 빌드 12회) 운영 사이클 학습

## 📁 프로젝트 구조

```
ShapeKeeper/
├── Assets/
│   ├── Another Assets/             # 외부 타일셋 등 외부 에셋
│   ├── Font/                       # 폰트 (TMP SDF 포함)
│   ├── Plugins/
│   │   └── Demigiant/DOTween/      # DOTween 트위닝 라이브러리
│   ├── Resources/
│   │   ├── 01.Scenes/              # Home / Plannet / Shop / Unit / Achievement / Setting
│   │   ├── 02.Script/
│   │   │   ├── Achieve/            # 업적 로딩 / 출석 체크
│   │   │   ├── Combine/            # 합성 보드 · 도감 · 필드 · 옵저버
│   │   │   ├── DB/                 # SQLite 연결, 업적 컨트롤러
│   │   │   ├── Data/               # Singleton, DataStorage, User, Unit, Enemy, Stamina, Quest, Mission, GridNode
│   │   │   ├── Enemy/              # 스폰, 이동, HP, 골(Goal) 판정, 미션 진행
│   │   │   ├── Game Controll/      # 게임 시작/종료 흐름
│   │   │   ├── Observer/           # 업적 / 날짜 변경 / 스태미나 옵저버
│   │   │   ├── Pooling/            # Bullet · Enemy · Effect · Card · Combine · Area · LockOn · Ora 풀
│   │   │   ├── Setter/             # 각 씬/패널 초기화 (Home, Plannet, Shop, Unit Page, Upgrade …)
│   │   │   ├── Shop/               # 상자/에너지 구매, 로딩, 통합 세팅
│   │   │   ├── Spell/              # 생성 / 발동 / 인식 / 강화 / 타겟팅
│   │   │   ├── Summon/             # 카드 → 타워 소환, 이동, 회수, 타입 변환, 공격, 총알 이동
│   │   │   ├── Tower Effect/       # 타워 시각 효과
│   │   │   └── UI Effect/          # UI 연출 (Stage Selecter, Plannet Hovering, Satellite Mover 등)
│   │   ├── 03.Prefabs/             # 런타임 로드 프리팹
│   │   └── 04.Sprite/              # 스프라이트 (Icon, Field, Unit, Enemy 등)
│   ├── Settings/                   # URP 설정 (Renderer / Volume / Global)
│   ├── StreamingAssets/
│   │   └── ShapeKeeperDB.db        # SQLite 게임 데이터베이스
│   └── TextMesh Pro/               # TMP 에셋
├── Packages/                       # Unity 패키지 매니페스트
└── ProjectSettings/                # Unity 프로젝트 설정
```

Android 서명 키(`*.keystore`)는 보안상 저장소에 포함되지 않습니다. 빌드 시 별도로 관리해야 합니다.

## 🚀 빌드 및 실행

> 이 저장소는 Git LFS를 사용합니다. 클론 전에 LFS가 설치되어 있어야 합니다.

```bash
git lfs install
git clone https://github.com/cacaru/ShapeKeeper.git
```

1. Unity Hub에서 **6000.0.63f1** 버전으로 프로젝트를 엽니다.
2. `Assets/Resources/01.Scenes/Home.unity`에서 게임 시작이 가능합니다.
3. Android 빌드 시 별도로 보관된 서명 키스토어를 `Project Settings > Player > Publishing Settings`에 지정합니다 (키 파일 및 비밀번호는 저장소에 포함되지 않음).

## 📦 주요 의존성

* `com.unity.feature.2d` 2.0.1
* `com.unity.render-pipelines.universal` 17.0.4
* `com.unity.inputsystem` 1.16.0
* `com.unity.ugui` 2.0.0
* `com.unity.timeline` 1.8.9
* `Mono.Data.Sqlite` (플러그인)
* DOTween (Demigiant)

## 📄 라이선스

본 프로젝트는 **SunhosWorld**의 자산입니다. All Rights Reserved.
