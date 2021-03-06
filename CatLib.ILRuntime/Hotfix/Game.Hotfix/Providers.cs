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

using CatLib;
using Game.Hotfix.TestsAlias;
using Game.Hotfix.TestsCanMake;
using Game.Hotfix.TestsExtend;
using Game.Hotfix.TestsGetBind;
using Game.Hotfix.TestsHasBind;
using Game.Hotfix.TestsHasInstance;
using Game.Hotfix.TestsIsAlias;
using Game.Hotfix.TestsIsResolved;
using Game.Hotfix.TestsIsStatic;

namespace Game.Hotfix
{
    /// <summary>
    /// 项目注册的服务提供者
    /// </summary>
    public static class Providers
    {
        /// <summary>
        /// 项目注册的服务提供者
        /// </summary>
        public static IServiceProvider[] ServiceProviders => new IServiceProvider[]
        {
            new ProviderHasInstance(), 
            new ProviderGetBind(), 
            new ProviderIsResolved(), 
            new ProviderHasBind(), 
            new ProviderCanMake(), 
            new ProviderIsStatic(),
            new ProviderIsAlias(), 
            new ProviderAlias(), 
            new ProviderExtend(), 
        };
    }
}