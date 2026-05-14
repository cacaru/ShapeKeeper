# ShapeKeeper

> 카드로 타워를 소환하고 자유롭게 배치하여, 기지를 향해 진격하는 적들을 막아내는 **타워 배치형 디펜스 게임**

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.0.63f1-000000?logo=unity" alt="Unity 6">
  <img src="https://img.shields.io/badge/Render%20Pipeline-URP-2088FF" alt="URP">
  <img src="https://img.shields.io/badge/Language-C%23-239120?logo=csharp" alt="C#">
  <img src="https://img.shields.io/badge/Platform-Android-3DDC84?logo=android" alt="Android">
  <img src="https://img.shields.io/badge/Version-1.3.1-blue" alt="version">
</p>

<!-- TODO: 대표 이미지 / GIF 삽입
<p align="center">
  <img src="docs/images/hero.gif" width="600">
</p>
-->

---

## 🎮 플레이하기

<!-- TODO: 출시된 플랫폼 링크 추가 -->
- **Google Play**: <!-- https://play.google.com/store/apps/details?id=... -->
- **APK 다운로드**: <!-- Release 페이지 링크 -->

## ✨ 주요 특징

- **카드 기반 타워 소환** — 보유한 카드 덱에서 타워를 뽑아 전장에 배치
- **자유로운 타워 배치** — 정해진 슬롯이 아닌, 플레이어가 원하는 위치에 자유 배치
- **타워 합성(Combine)** — 동일 타워를 합쳐 상위 등급으로 강화하는 머지 메커닉
- **스펠 시스템** — 전투 중 즉발형 스킬로 위기 상황 돌파
- **업적 & 출석 보상** — 장기 플레이 동기를 주는 메타 진행 시스템
- **상점 / 재화 관리** — 카드/재화/업그레이드 경제 루프

## 🛠 기술 스택

| 분야 | 사용 기술 |
| --- | --- |
| 엔진 | Unity 6 (6000.0.63f1) |
| 렌더링 | Universal Render Pipeline (URP) |
| 언어 | C# |
| 입력 | Unity New Input System |
| 트위닝 | DOTween |
| 빌드 타겟 | Android (IL2CPP) |
| 버전 관리 | Git + Git LFS |

## 🧩 핵심 구현

### Object Pooling
총알, 적, 이펙트, 카드, 락온 마커 등 빈번하게 생성/파괴되는 오브젝트를 **타입별 풀**로 분리해 GC 부하와 프레임 드랍을 최소화했습니다.
- `Bullet_Pool`, `Enemy_Pool`, `Effect_Pooling`, `Unit_Card_Pool`, `Combine_Pool`, `Area_Pool`, `Lock_On_Pool`, `Ora_Pool`

### 카드 → 소환 파이프라인
카드를 드래그하면 필드 좌표를 검사 → 타워 인스턴스화 → 카드 소비까지 단일 흐름으로 처리합니다.
- `Card_To_Summon` → `TowerInstaller` → `Summon`
- 같은 흐름에 **이동(Shift)**, **회수(Recall)**, **특수 유닛 변환** 기능을 확장해 카드 한 장으로 다양한 인터랙션을 표현

### 적 스폰 & 웨이브 제어
스테이지별 스포너가 시간/조건 기반으로 적을 출격시키고, 골(기지) 도달 시 미션 실패 판정을 트리거합니다.
- `Spawner`, `Goal_Spawner`, `EnemyMover`, `EnemyHP`, `MIssion_Carry_Out`

### 메타 진행 시스템
- **업적 로딩/저장** (`Achievement_Loader`, `Achieve_Loading`)
- **출석 체크 보상** (`Attendence_Checker`)
- **상점 / 재화** (`Shop/`, `DB/`, `Data/`)

## 📁 프로젝트 구조

```
Assets/
├─ Resources/
│  └─ 02.Script/
│     ├─ Achieve/        # 업적, 출석 체크
│     ├─ Combine/        # 타워 합성 시스템
│     ├─ DB/  Data/      # 세이브 / 마스터 데이터
│     ├─ Enemy/          # 적 스폰, 이동, HP, 골 판정
│     ├─ Game Controll/  # 게임 시작/종료 흐름
│     ├─ Observer/       # 이벤트 관찰자 패턴
│     ├─ Pooling/        # 오브젝트 풀
│     ├─ Setter/         # 초기화 / 세팅
│     ├─ Shop/           # 상점
│     ├─ Spell/          # 스펠 / 스킬
│     ├─ Summon/         # 카드 → 타워 소환, 이동, 회수
│     ├─ Tower Effect/   # 타워 시각 효과
│     └─ UI Effect/      # UI 연출
├─ Another Assets/        # 외부 에셋 (타일셋 등)
├─ Plugins/               # 서드파티 (DOTween 등)
└─ Settings/              # URP 설정
```

## 👤 개발 정보

- **개발자**: Cacau (SunhosWorld)
- **장르**: Tower Defense / Card Strategy
- **개발 기간**: <!-- TODO: 예) 2025.02 ~ 2025.12 -->
- **개발 인원**: <!-- TODO: 1인 개발 / 팀 N명 -->
- **담당 역할**: <!-- TODO: 기획·프로그래밍·디자인 등 본인 담당 -->

## 🚀 빌드 및 실행

> 이 저장소는 Git LFS를 사용합니다. 클론 전에 LFS가 설치되어 있어야 합니다.

```bash
git lfs install
git clone https://github.com/cacaru/ShapeKeeper.git
```

1. Unity Hub에서 **6000.0.63f1** 버전 설치
2. Unity Hub → **Open** → 클론한 폴더 선택
3. 빌드 시 Android 모듈 필요

> Android 빌드를 위한 keystore 파일은 보안상 저장소에 포함되어 있지 않습니다.

## 🖼 스크린샷

<!-- TODO: 게임 플레이 스크린샷 3~6장 추가
| 메인 메뉴 | 인게임 | 합성 |
| :---: | :---: | :---: |
| ![](docs/images/menu.png) | ![](docs/images/play.png) | ![](docs/images/combine.png) |
-->

## 📄 라이선스

© 2025 SunhosWorld. All Rights Reserved.
