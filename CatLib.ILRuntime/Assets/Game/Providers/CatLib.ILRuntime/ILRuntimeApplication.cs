﻿/*
 * This file is part of the CatLib package.
 *
 * (c) CatLib <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://catlib.io/
 */

using System;
using System.Collections;
using System.Collections.Generic;
using CatLib.API.ILRuntime;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

namespace CatLib.ILRuntime
{
    /// <summary>
    /// ILRuntime Application
    /// </summary>
    public class ILRuntimeApplication : UnityApplication
    {
        /// <summary>
        /// App Domain
        /// </summary>
        private IAppDomain appDomain;

        /// <summary>
        /// 是否推迟初始化
        /// </summary>
        private bool deferInit;

        /// <summary>
        /// 延迟的服务提供者列表
        /// </summary>
        private readonly SortedList<int, List<IServiceProvider>> deferServiceProviders
            = new SortedList<int, List<IServiceProvider>>();

        /// <summary>
        /// Appdomain
        /// </summary>
        protected IAppDomain AppDomain
        {
            get { return appDomain ?? (appDomain = App.Make<IAppDomain>()); }
        }

        /// <summary>
        /// 构建一个ILRuntime Application实例
        /// </summary>
        /// <param name="behaviour">Unity主MonoBehaviour</param>
        public ILRuntimeApplication(MonoBehaviour behaviour)
            : base(behaviour)
        {
            deferInit = false;
            App.Extend<ILTypeInstance>(instance => instance.CLRInstance);
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="makeServiceType">需求构建的类型</param>
        /// <param name="userParams">构造函数参数信息</param>
        /// <returns>创建的实例</returns>
        protected override object CreateInstance(Type makeServiceType, object[] userParams)
        {
            if (makeServiceType is ILRuntimeType || makeServiceType is ILRuntimeWrapperType)
            {
                return AppDomain.CreateInstance(makeServiceType.FullName, userParams);
            }

            return base.CreateInstance(makeServiceType, userParams);
        }

        /// <summary>
        /// 注册服务提供者
        /// </summary>
        /// <param name="provider">服务提供者</param>
        public override void Register(IServiceProvider provider)
        {
            if (!deferInit)
            {
                base.Register(provider);
                return;
            }
            StartCoroutine(CoroutineRegister(provider));
        }

        /// <summary>
        /// 延缓初始化服务提供者，直到闭包中的服务提供者全部完成注册
        /// </summary>
        public void DeferInitServiceProvider(Action closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);

            if (!IsMainThread)
            {
                throw new CodeStandardException("Must be call DeferInitServiceProvider in the main thread.");
            }

            if (deferInit)
            {
                throw new CodeStandardException("Already in an DeferInitServiceProvider environment");
            }

            try
            {
                deferInit = true;
                closure();
            }
            finally
            {
                deferInit = false;
                StartCoroutine(RestoreDeferServiceProviders());
            }
        }

        /// <summary>
        /// 恢复顺延初始化的服务提供者
        /// </summary>
        private IEnumerator RestoreDeferServiceProviders()
        {
            if (deferServiceProviders.Count <= 0)
            {
                yield break;
            }

            foreach (var sorted in deferServiceProviders)
            {
                foreach (var provider in sorted.Value)
                {
                    yield return InitProvider(provider);
                }
            }
        }

        /// <summary>
        /// 初始化服务提供者
        /// </summary>
        /// <param name="provider">服务提供者</param>
        /// <returns></returns>
        protected override IEnumerator InitProvider(IServiceProvider provider)
        {
            if (deferInit)
            {
                AddSortedList(deferServiceProviders, provider, "Init");
                yield break;
            }
            yield return base.InitProvider(provider);
        }
    }
}
