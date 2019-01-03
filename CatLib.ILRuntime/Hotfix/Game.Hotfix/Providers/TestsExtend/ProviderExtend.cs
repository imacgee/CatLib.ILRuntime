/*
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
using Game.Hotfix.API.TestsExtend;
using System.Collections;
using UnityEngine;
using static CatLib.App;

namespace Game.Hotfix.TestsExtend
{
    public class ProviderExtend : ServiceProvider
    {
        public override void Init()
        {
            //var s = Make("s");
            //Debug.Log("123:"+s);
            Util.Log("Init() : ProviderAlias");
            var extend = Make<IExtend>();
            Debug.Log(extend.Name);
            if (extend.Name != "Extend")
            {
                Util.Faild("Extend_1");
                return;
            }

            Util.Success("Extend"); 
        }

        public override IEnumerator CoroutineInit()
        {
            Util.Log("CoroutineInit() : ProviderExtend");
            return base.CoroutineInit();
        }

        public override void Register()
        {
            //Bind("s", (c, p) => new object(), true);
            
            Singleton<IExtend, Extend>();
            Extend<IExtend, IExtend>((instance, c) =>
            {
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!");
                Debug.Log(instance);
                instance.Name = "Extend";
                return instance;
            });
        }
    }
}
