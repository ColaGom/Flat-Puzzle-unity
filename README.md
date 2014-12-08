# Flat puzzle project. 
_Copyright (c) 2014 ColabearStudio All rights reserved._

유니티 엔진을 활용하여 제작된 간단한 3-match 형식의 오픈 소스 게임.

## 개요

시스템 환경.

* Unity 4.5.4 pro
* Android SDK - Google play service lib.

사용 플러그인 & 광고 플렛폼.

* [NGUI 3.4.9](http://www.tasharen.com/)
* [Google play game service for unity3d](https://github.com/playgameservices/play-games-plugin-for-unity)
* [Soomla plugin](soom.la)
* [google ads plugin](https://github.com/googleads/googleads-mobile-plugins)
* [adbuddiz](www.adbuddiz.com)


## 구현내용

* 3-match 형식의 기본적인 게임 로직 구현.
* 싱글플레이
* 멀티플레이(Google play game service plugin을 활용한 1:1 자동 매칭 실시간 대전 방식.)
* 리더보드
* 업적
* 상점(IAP)
* 광고적용
* 레벨시스템적용.
* 안드로이드 .jar 파일을 활용한 다이얼로그 구현.

## 업데이트 예정

 * 아이템.
 * 게임내 재화 구현.
 * 소셜기능 구현 ( Facebook , Twitter. )


## 클래스 설명

 * MatchContainer - 3-match puzzle 게임로직이 담겨있는 클래스.
 * 각종 Manager 클래스 - 해당하는 기능들을 구현해둔 클래스.
 * Helper 클래스 - 광고 , 인앱결제기능을 구현해둔 클래스.
 * MainFrame - Application 동작의 전체적인 흐름을 제어하는 클래스.
 * 추후 추가 작성...

## 추가 전달사항.
 * Google , 광고 사용을 위한 각종 key 값들은 지워둔 상태입니다.
 * 아직 주석작업을 하지 못하였습니다.
