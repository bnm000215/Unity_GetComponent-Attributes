# Unity_GetComponentAttributes
 - ```GetComponent()``` 종류의 메소드들을 필드/프로퍼티 애트리뷰트로 제공합니다.
 
<br>

## Preview
![image](https://user-images.githubusercontent.com/42164422/78679874-bb384780-7925-11ea-9975-186a3ef34c24.png)

<br>

## GetComponent Attributes
 ### [1] 설명
  - 리플렉션과 커스텀 애트리뷰트를 활용하여 제작하였습니다.
  - ```Component```를 상속받는 타입의 필드/프로퍼티에 사용할 수 있습니다.
  - 본 애트리뷰트들을 통한 컴포넌트 할당 기능은 OnEnable() 이후, Start() 이전에 동작합니다.
 
  <br>
  
 ### [2] 주의사항
  - public 멤버들에 대해서만 동작합니다.
  - ```Component``` 클래스를 상속하는 타입들에 대해서만 동작합니다.
  - ```[GetComponent]```, ```[GetComponentInChildren]```, ```[GetComponentInParent]```, ```[GetOrAddComponent]```, ```[GetOrAddComponentInChildren]```, ```[GetOrAddComponentInParent]``` 애트리뷰트는 ```Array```나 ```List```, ```Dictionary``` 등 컨테이너 또는 제네릭 타입의 멤버에 대해 동작하지 않습니다.
  - ```[GetComponents]```, ```[GetComponentsInChildren]```, ```[GetComponentsInParent]``` 애트리뷰트는
  <br>요소의 타입이 ```Component```를 상속하는 경우의 ```Array``` 또는 ```List``` 타입 멤버에 대해 동작합니다.
  - ```GetComponentController``` 클래스가 씬 내에 활성화된 컴포넌트의 형태로 존재하는 경우에만 모든 기능이 올바르게 동작 합니다.
  
  <br>
  
 ### [3] 애트리뷰트 종류
  **(공통 매개변수 : ```AllowOverwrite```(bool) -> 기본 값은 ```false```)**
  <br> -> ```false```일 경우 : 값이 초기화되지 않아 null인 상태의 필드/프로퍼티에 대해서만 동작합니다.
  <br> -> ```true```일 경우 : 기존에 값이 초기화된 필드/프로퍼티에 대해서도 모두 동작합니다.
 
  - ```[GetComponent]```
   <br>: 게임오브젝트 내에서 해당 타입의 컴포넌트를 찾아 멤버 변수에 초기화합니다.
   <br>
   
  - ```[GetComponentInChildren]```
   <br>: 자기 또는 자식 게임오브젝트들 내에서 해당 타입의 컴포넌트를 찾아 멤버 변수에 초기화합니다.
   <br>
   
  - ```[GetComponentInAChild]```
   <br>: 지정한 이름의 자식 게임오브젝트에서 해당 타입의 컴포넌트를 찾아 지정된 필드 또는 프로퍼티에 초기화합니다.
   <br>* 해당 이름의 자식 게임오브젝트가 존재하지 않을 경우, 아무런 동작을 하지 않습니다.
   <br>
   
  - ```[GetComponentInParent]```
   <br>: 자기 또는 부모 게임오브젝트들 내에서 해당 타입의 컴포넌트를 찾아 멤버 변수에 초기화합니다.
   <br>
  
  - ```[GetComponents]```
   <br>: 게임오브젝트 내에서 해당 타입의 컴포넌트들을 모두 찾아 멤버 변수에 초기화합니다.
   <br>* *Array, List에 대해서만 동작합니다.*
   <br>
   
  - ```[GetComponentsInChildren]```
   <br>: 자기 또는 자식 게임오브젝트들 내에서 해당 타입의 컴포넌트들을 모두 찾아 멤버 변수에 초기화합니다.
   <br>* *Array, List에 대해서만 동작합니다.*
   <br>
   
  - ```[GetComponentsInParent]```
   <br>: 자기 또는 부모 게임오브젝트들 내에서 해당 타입의 컴포넌트들을 모두 찾아 멤버 변수에 초기화합니다.
   <br>* *Array, List에 대해서만 동작합니다.*
   <br>
  
  - ```[GetOrAddComponent]```
   <br>: 게임오브젝트 내에서 해당 타입의 컴포넌트를 찾아 멤버 변수에 초기화
   <br>* **해당 컴포넌트가 존재하지 않을 경우**, 게임오브젝트 내에 생성 및 추가하여 멤버 변수에 초기화합니다.
   <br>
   
  - ```[GetOrAddComponentInChildren]```
   <br>: 자기 또는 자식 게임오브젝트들 내에서 해당 타입의 컴포넌트를 찾아 멤버 변수에 초기화합니다.
   <br><br>* **해당 컴포넌트가 존재하지 않을 경우**
   >> (1) 매개변수로 지정한 이름의 자식 게임오브젝트를 탐색합니다.
   <br> (2) 해당 이름의 자식 게임오브젝트가 존재할 경우,
   <br>     해당 게임오브젝트에 컴포넌트를 생성/추가하여 멤버 변수에 초기화합니다.
   <br> (3) **해당 이름의 자식 게임오브젝트가 존재하지 않을 경우**, 
   <br>     해당 이름으로 자식 게임오브젝트를 생성한 뒤 컴포넌트를 생성 및 추가하여 멤버 변수에 초기화합니다.
   <br>
   
  - ```[GetOrAddComponentInParent]```
   <br>: 자기 또는 부모 게임오브젝트들 내에서 해당 타입의 컴포넌트를 찾아 멤버 변수에 초기화
   <br><br>* **해당 컴포넌트가 존재하지 않을 경우**
   >> (1) 매개변수로 지정한 이름의 부모 게임오브젝트를 탐색합니다.
   <br> (2) 해당 이름의 부모 게임오브젝트가 존재할 경우, 
   <br>     해당 게임오브젝트에 컴포넌트를 생성 및 추가하여 멤버 변수에 초기화합니다.
   <br> (3) 해당 이름의 부모 게임오브젝트가 존재하지 않을 경우 아무런 동작을 하지 않습니다.
  
  <br>
  
 ### [4] 실행 화면
  
