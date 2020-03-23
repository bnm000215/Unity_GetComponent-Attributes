﻿/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace Rito
{
    // 2020. 03. 18. 작성
    // 2020. 03. 20. 필드 - Array 대상 구현 및 테스트 완료
    // 2020. 03. 20. 필드 - List 대상 구현 및 테스트 완료
    // 2020. 03. 21. 필드 - GetOrAddComponent 구현 및 테스트 완료
    // 2020. 03. 21. 프로퍼티 대상으로 구현 완료

    // T o D o ★ : 필드 & 프로퍼티 대상 메소드 합칠 수 있도록
    // 1. 각 부분을 작은 메소드화
    // 2. 하나의 메소드로 합치기

    // TODO : 모든 테스트케이스 검증

    // TODO : 에디터를 실행하면 알아서 이 싱글톤 오브젝트가 만들어지게 하기 ★
    
    public class GetComponentController : MonoBehaviour
    {
        #region Singleton - Public

        /// <summary> 싱글톤 인스턴스 Getter </summary>
        public static GetComponentController Instance
        {
            get
            {
                if (instance == null)    // 체크 1 : 인스턴스가 없는 경우
                    CheckExsistence();

                return instance;
            }
        }

        /// <summary> 싱글톤 인스턴스의 또다른 이름 </summary>
        public static GetComponentController Sin => Instance;
        /// <summary> 싱글톤 인스턴스의 또다른 이름 </summary>
        public static GetComponentController Ins => Instance;
        /// <summary> 싱글톤 인스턴스의 또다른 이름 </summary>
        public static GetComponentController I => Instance;

        /// <summary>
        /// 싱글톤을 그저 생성하기 위한 메소드
        /// </summary>
        public void Call() { }

        /// <summary>
        /// 싱글톤을 그저 생성하기 위한 정적 메소드
        /// </summary>
        public static void Call_()
        {
            if (instance == null)
                CheckExsistence();
        }

        #endregion // ==================================================================

        #region Singleton - Private

        // 싱글톤 인스턴스
        private static GetComponentController instance;

        // 싱글톤 인스턴스 존재 여부 확인 (체크 2)
        private static void CheckExsistence()
        {
            // 싱글톤 검색
            instance = FindObjectOfType<GetComponentController>();

            // 인스턴스 가진 오브젝트가 존재하지 않을 경우, 빈 오브젝트를 임의로 생성하여 인스턴스 할당
            if (instance == null)
            {
                // 빈 게임 오브젝트 생성
                GameObject container = new GameObject("GetComponentController Singleton Container");

                // 게임 오브젝트에 클래스 컴포넌트 추가 후 인스턴스 할당
                instance = container.AddComponent<GetComponentController>();
            }
        }

        /// <summary> 
        /// [Awake()에서 호출]
        /// <para/> 싱글톤 스크립트를 미리 오브젝트에 담아 사용하는 경우를 위한 로직
        /// </summary>
        private void CheckInstance()
        {
            // 싱글톤 인스턴스가 존재하지 않았을 경우, 본인으로 초기화
            if (instance == null)
                instance = this;

            // 싱글톤 인스턴스가 존재하는데, 본인이 아닐 경우, 스스로를 파괴
            if (instance != null && instance != this)
                Destroy(this);
        }

        #endregion // ==================================================================

        private void Awake()
        {
            CheckInstance();

            AttributeAction(EventFlow.Awake);
        }

        private void Start()
        {
            AttributeAction(EventFlow.Start);
        }

        /// <summary> 원하는 유니티 기본 이벤트 타이밍에 GetComponent 동작 </summary>
        private void AttributeAction(EventFlow flow)
        {
            // 모든 활성 컴포넌트 찾기
            var allComponents = FindObjectsOfType<Component>();

            foreach (var component in allComponents)
            {
                // 1. 필드 대상으로 동작
                GetComponentActionInFields(component, flow);

                // 2. 프로퍼티 대상으로 동작
                GetComponentActionInProperties(component, flow);
            }
        }

        /// <summary> 필드 대상으로 GetComponent 기능 동작 </summary>
        private void GetComponentActionInFields(Component p_component, EventFlow p_flow)
        {
            // 1. 모든 필드 찾기 -> NonPublic은 타입 찾을 때 인식이 안되니 미리 패스
            var fInfos = p_component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            // 2. Component의 자식 타입인 참조형 필드만 골라내기 (is Component)
            // 3. GetComponent류 애트리뷰트가 있는 필드만 골라내기
            // 4. 입력된 이벤트 타이밍이 일치하는 필드만 골라내기
            var targetFInfos =
                from fieldInfo in fInfos
                where fieldInfo.GetCustomAttribute<GetComponentAttributeBase>()?.Flow == p_flow &&
                      (fieldInfo.GetValue(p_component) is Component ||
                       fieldInfo.GetValue(p_component) is Component[] ||
                       (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                      )
                select fieldInfo;

            // 5. GetComponent 종류별로 해주기
            foreach (var fInfo in targetFInfos)
            {
                GameObject go = p_component.gameObject;
                var customAttribute = fInfo.GetCustomAttribute<GetComponentAttributeBase>();
                var fieldType = fInfo.FieldType;
                var fieldValue = fInfo.GetValue(p_component);
                string methodName = "";
                Type[] methodParams = new Type[0];      // GetOrAdd에서 사용 : 메소드 파라미터
                object[] realParams = new object[0];    // GetOrAdd에서 사용 : 실질 파라미터
                string goaTargetString = "";            // GetOrAdd 애트리뷰트에서 가져오는 타겟 부모 또는 자식 이름 스트링

                // 5-1. GetComponent, GetOrAddComponent
                if (fieldValue is Component)
                {
                    // GetComponent, GetOrAdd
                    switch (customAttribute)
                    {
                        case GetComponent g:
                            fInfo.SetValue(p_component, go.GetComponent(fieldType));
                            break;

                        case GetComponentInParent g:
                            fInfo.SetValue(p_component, go.GetComponentInParent(fieldType));
                            break;

                        case GetComponentInChildren g:
                            fInfo.SetValue(p_component, go.GetComponentInChildren(fieldType));
                            break;


                        case GetOrAddComponent goa:
                            methodName = "GetOrAddComponent";
                            methodParams = new Type[] { typeof(GameObject), typeof(Type) };
                            realParams = new object[] { p_component.gameObject, fieldType };
                            break;

                        case GetOrAddComponentInChildren goa:
                            methodName = "GetOrAddComponentInChildren";
                            goaTargetString = (customAttribute as GetOrAddComponentInChildren).ChildObjectName;
                            methodParams = new Type[] { typeof(GameObject), typeof(Type), typeof(string) };
                            realParams = new object[] { p_component.gameObject, fieldType, goaTargetString };
                            break;

                        case GetOrAddComponentInParent goa:
                            methodName = "GetOrAddComponentInParent";
                            goaTargetString = (customAttribute as GetOrAddComponentInParent).ParentObjectName;
                            methodParams = new Type[] { typeof(GameObject), typeof(Type), typeof(string) };
                            realParams = new object[] { p_component.gameObject, fieldType, goaTargetString };
                            break;
                    }

                    // Get Or Add
                    if (methodName.Length > 0)
                    {
                        var getOrAddMethod = typeof(GetComponentExtension).GetMethod(methodName, methodParams);
                        var returnedComponent = getOrAddMethod.Invoke(p_component.gameObject, realParams);
                        fInfo.SetValue(p_component, returnedComponent);
                    }
                }
                // Array or List
                else
                {
                    switch (customAttribute)
                    {
                        case GetComponents getComs:
                            methodName = "GetComponents";
                            break;

                        case GetComponentsInParent getComsParent:
                            methodName = "GetComponentsInParent";
                            break;

                        case GetComponentsInChildren getComsChildren:
                            methodName = "GetComponentsInChildren";
                            break;
                    }

                    // 5-2. GetComponents - Array
                    if (fieldValue is Component[])
                    {
                        Type elementType = fieldType.GetElementType();

                        if (methodName.Length > 0)
                        {
                            // GetCompo~<타입> 메소드를 타입 지정하여 가져오기
                            MethodInfo getComsMethod = typeof(GameObject).GetMethod(methodName, new Type[0])
                                        .MakeGenericMethod(elementType);

                            // 게임오브젝트로부터 해당 타입의 컴포넌트들 가져오기
                            var targetComponentsObj = getComsMethod.Invoke(go, null);

                            // 배열로 변환
                            Array targetComponentsArr = targetComponentsObj as Array;

                            // 타겟 필드에 할당
                            if (targetComponentsArr != null)
                                fInfo.SetValue(p_component, targetComponentsArr);
                        }
                    }

                    // 5-3. GetComponents - List<T>
                    else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        // T 타입 구하기
                        Type genericType = fieldType.GetGenericArguments()[0];

                        if (methodName.Length > 0)
                        {
                            // GetCompo~<타입> 메소드를 타입 지정하여 가져오기
                            MethodInfo getComsMethod = typeof(GameObject).GetMethod(methodName, new Type[0])
                                        .MakeGenericMethod(genericType);

                            // 게임오브젝트로부터 해당 타입의 컴포넌트들 가져오기
                            var targetComponentsObj = getComsMethod.Invoke(go, null);

                            // 배열로 변환
                            Array targetComponentsArr = targetComponentsObj as Array;

                            if (targetComponentsArr != null)
                            {
                                // 새로운 리스트를 인스턴스화하여 생성
                                object newList = Activator.CreateInstance(fieldType);

                                // Add 메소드 가져오기
                                MethodInfo AddToListMethod = fieldType.GetMethod("Add");

                                // 리스트에 배열 요소들 초기화
                                foreach (var item in targetComponentsArr)
                                {
                                    AddToListMethod.Invoke(newList, new object[] { item });
                                }

                                // 타겟 필드에 리스트 참조 할당
                                fInfo.SetValue(p_component, newList);
                            }
                        } // if (methodName.Length > 0)
                    } // else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                } // else (Array or List)
            } // foreach (var field in targetFields)
        } // GinF Method End

        /// <summary> 프로퍼티 대상으로 GetComponent 기능 동작 </summary>
        private void GetComponentActionInProperties(Component p_component, EventFlow p_flow)
        {
            var pInfos = p_component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var targetPInfos =
                from pInfo in pInfos
                where pInfo.SetMethod != null && // 프로퍼티에 Setter가 없는 경우 X
                      pInfo.GetCustomAttribute<GetComponentAttributeBase>()?.Flow == p_flow &&
                      (pInfo.PropertyType.IsSubclassOf(typeof(Component)) || pInfo.PropertyType.IsEquivalentTo(typeof(Component)) ||
                       pInfo.PropertyType.IsSubclassOf(typeof(Array)) && pInfo.PropertyType.GetElementType().IsSubclassOf(typeof(Component)) ||
                       pInfo.PropertyType.IsEquivalentTo(typeof(Component[])) ||
                       (pInfo.PropertyType.IsGenericType && pInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                      )
                select pInfo;

            // 5. GetComponent 종류별로 해주기
            foreach (var pInfo in targetPInfos)
            {
                GameObject go = p_component.gameObject;
                var customAttribute = pInfo.GetCustomAttribute<GetComponentAttributeBase>();
                var propertyType = pInfo.PropertyType;
                var propertyValue = pInfo.GetValue(p_component);
                string methodName = "";
                Type[] methodParams = new Type[0];      // GetOrAdd에서 사용 : 메소드 파라미터
                object[] realParams = new object[0];    // GetOrAdd에서 사용 : 실질 파라미터
                string goaTargetString = "";            // GetOrAdd 애트리뷰트에서 가져오는 타겟 부모 또는 자식 이름 스트링

                // 5-1. GetComponent, GetOrAddComponent
                if (propertyType.IsSubclassOf(typeof(Component)) || propertyType.IsEquivalentTo(typeof(Component)))
                {
                    // GetComponent, GetOrAdd
                    switch (customAttribute)
                    {
                        case GetComponent g:
                            pInfo.SetValue(p_component, go.GetComponent(propertyType));
                            break;

                        case GetComponentInParent g:
                            pInfo.SetValue(p_component, go.GetComponentInParent(propertyType));
                            break;

                        case GetComponentInChildren g:
                            pInfo.SetValue(p_component, go.GetComponentInChildren(propertyType));
                            break;


                        case GetOrAddComponent goa:
                            methodName = "GetOrAddComponent";
                            methodParams = new Type[] { typeof(GameObject), typeof(Type) };
                            realParams = new object[] { p_component.gameObject, propertyType };
                            break;

                        case GetOrAddComponentInChildren goa:
                            methodName = "GetOrAddComponentInChildren";
                            goaTargetString = (customAttribute as GetOrAddComponentInChildren).ChildObjectName;
                            methodParams = new Type[] { typeof(GameObject), typeof(Type), typeof(string) };
                            realParams = new object[] { p_component.gameObject, propertyType, goaTargetString };
                            break;

                        case GetOrAddComponentInParent goa:
                            methodName = "GetOrAddComponentInParent";
                            goaTargetString = (customAttribute as GetOrAddComponentInParent).ParentObjectName;
                            methodParams = new Type[] { typeof(GameObject), typeof(Type), typeof(string) };
                            realParams = new object[] { p_component.gameObject, propertyType, goaTargetString };
                            break;
                    }

                    // Get Or Add
                    if (methodName.Length > 0)
                    {
                        var getOrAddMethod = typeof(GetComponentExtension).GetMethod(methodName, methodParams);
                        var returnedComponent = getOrAddMethod.Invoke(p_component.gameObject, realParams);
                        pInfo.SetValue(p_component, returnedComponent);
                    }
                }
                // Array or List
                else
                {
                    switch (customAttribute)
                    {
                        case GetComponents getComs:
                            methodName = "GetComponents";
                            break;

                        case GetComponentsInParent getComsParent:
                            methodName = "GetComponentsInParent";
                            break;

                        case GetComponentsInChildren getComsChildren:
                            methodName = "GetComponentsInChildren";
                            break;
                    }

                    // 5-2. GetComponents - Array
                    if (propertyType.IsSubclassOf(typeof(Array)) || propertyType.IsEquivalentTo(typeof(Component[])))
                    {
                        Type elementType = propertyType.GetElementType();

                        if (methodName.Length > 0)
                        {
                            // GetCompo~<타입> 메소드를 타입 지정하여 가져오기
                            MethodInfo getComsMethod = typeof(GameObject).GetMethod(methodName, new Type[0])
                                        .MakeGenericMethod(elementType);

                            // 게임오브젝트로부터 해당 타입의 컴포넌트들 가져오기
                            var targetComponentsObj = getComsMethod.Invoke(go, null);

                            // 배열로 변환
                            Array targetComponentsArr = targetComponentsObj as Array;

                            // 타겟 필드에 할당
                            if (targetComponentsArr != null)
                                pInfo.SetValue(p_component, targetComponentsArr);
                        }
                    }

                    // 5-3. GetComponents - List<T>
                    else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        // T 타입 구하기
                        Type genericType = propertyType.GetGenericArguments()[0];

                        if (methodName.Length > 0)
                        {
                            // GetCompo~<타입> 메소드를 타입 지정하여 가져오기
                            MethodInfo getComsMethod = typeof(GameObject).GetMethod(methodName, new Type[0])
                                        .MakeGenericMethod(genericType);

                            // 게임오브젝트로부터 해당 타입의 컴포넌트들 가져오기
                            var targetComponentsObj = getComsMethod.Invoke(go, null);

                            // 배열로 변환
                            Array targetComponentsArr = targetComponentsObj as Array;

                            if (targetComponentsArr != null)
                            {
                                // 새로운 리스트를 인스턴스화하여 생성
                                object newList = Activator.CreateInstance(propertyType);

                                // Add 메소드 가져오기
                                MethodInfo AddToListMethod = propertyType.GetMethod("Add");

                                // 리스트에 배열 요소들 초기화
                                foreach (var item in targetComponentsArr)
                                {
                                    AddToListMethod.Invoke(newList, new object[] { item });
                                }

                                // 타겟 필드에 리스트 참조 할당
                                pInfo.SetValue(p_component, newList);
                            }
                        }
                    }
                }
            }
        } // GinP Method End

    }
}
*/