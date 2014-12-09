# Flat puzzle project. 
_Copyright (c) 2014 ColabearStudio All rights reserved._

유니티 엔진을 활용하여 제작된 간단한 3-match 기본 게임 로직.

[App Download in PlayStore](https://play.google.com/store/apps/details?id=com.cbs.pang)

## 개요

시스템 환경.

* Unity 4.3.x 이상 

## 구현내용

* 3-match 형식의 기본적인 게임 로직 구현.
* 대기시 힌트 표시.
* 초기에 생성된 블럭들을 재활용 하여 게임진행. 추가생성 , 파괴 X
* 더이상 매치가 불가능할경우 전체 블록 재정렬.
* MatchContainer의 블록 witdh, height값 을 기준으로 camera orthographicSize 셋팅.

## 기본로직.

* Palette 클래스에 등록되어있는 색상을 기준으로 블록 생성.
* 2개의 블록 선택( 드래그 , 클릭 ) 
* 선택된 2개의 블록 스왑
* 스왑완료시 변경된 2개의 블록데이터 스왑 , 매칭 체크
* 매칭실패시 다시 원상태로 스왑.
* 매칭성공시 블록의 상태를 매치로 변경해준뒤 대기.
* 프레임별로 매치된 블럭이 존재할시 블럭 제거후 정렬.
* 재생성된 블록의 매치 여부 판별. 반복.

## 업데이트 예정

 * Umm..


## 클래스 설명

 * MatchContainer - 3-match puzzle 게임로직이 담겨있는 클래스.
 * 추후 추가 작성.

## 프로젝트 확인 방법.
 * 1.Project 다운로드
 * 2.실행후 Mainscene실행 & 확인.

## 추가 전달사항.
 * 원래 위의 스토어에 사용된 프로젝트를 통으로 올릴예정이였으나. 
 * 이것저것 라이센스 문제가 많이 보여서 게임로직만 따로 빼서 올립니다.
 * 아주 간단한 로직이긴하나 혹시나 필요한 분이 계실듯하여 올려봅니다.
 
## 추가 문의
 colabearstudio@gmail.com
