# ShapeKeeper

타워를 자유롭게 배치하여, 기지를 공격하러 오는 적을 막는 타워 배치형 디펜스 게임.

## 개발 환경

- **Unity**: 6000.0.63f1 (Unity 6)
- **렌더 파이프라인**: Universal Render Pipeline (URP)
- **입력**: New Input System

## 시작하기

1. 이 저장소를 클론합니다. (Git LFS 필요)
   ```bash
   git lfs install
   git clone https://github.com/<your-account>/ShapeKeeper.git
   ```
2. Unity Hub에서 `6000.0.63f1` 버전을 설치합니다.
3. Unity Hub → **Open** → 클론한 폴더를 선택해 엽니다.

## 프로젝트 구조

| 경로 | 설명 |
| --- | --- |
| `Assets/` | 게임 에셋 (스크립트, 프리팹, 머티리얼 등) |
| `Packages/` | Unity Package Manager 의존성 |
| `ProjectSettings/` | 프로젝트 설정 |
| `Library/`, `Temp/`, `Logs/`, `obj/` | Unity 자동 생성 (Git에서 제외됨) |

## 빌드

Android 빌드용 keystore(`shape_keeper_key.keystore`)는 보안상 저장소에 포함되어 있지 않습니다. 빌드가 필요한 경우 별도로 보관된 keystore 파일을 프로젝트 루트에 배치한 뒤 빌드하세요.

## 라이선스

All Rights Reserved.
