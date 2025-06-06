using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Share
{
    public interface IShare
    {
        UniTask TakeScreenshotAndShare();
    }
    
    public class ShareManager : MonoBehaviour, IShare, IService
    {
        [SerializeField] private string subject;
        [SerializeField] private string text;
        [SerializeField] private string url;
        [SerializeField] private string filename;
        public UniTask Initialize() => UniTask.CompletedTask;
        public UniTask PostInitialize() => UniTask.CompletedTask;
        
        #if USE_NATIVE_SHARE_PLUGIN
        public async UniTask TakeScreenshotAndShare()
        {
            await UniTask.WaitForEndOfFrame(this);

            var ss = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
            ss.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
            ss.Apply();

            var filePath = Path.Combine( Application.temporaryCachePath, filename );
            await File.WriteAllBytesAsync( filePath, ss.EncodeToPNG() );

            // To avoid memory leaks
            Destroy( ss );

            new NativeShare()
                .AddFile( filePath )
                .SetSubject( subject)
                .SetText(text)
                .SetUrl(url)
                .SetCallback( ( result, shareTarget ) => Debug.Log( "Share result: " + result + ", selected app: " + shareTarget ) )
                .Share();

            // Share on WhatsApp only, if installed (Android only)
            //if( NativeShare.TargetExists( "com.whatsapp" ) )
            //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
        }
        #else
        public UniTask TakeScreenshotAndShare()
        {
            throw new NotImplementedException("Use USE_NATIVE_SHARE_PLUGIN symbol to start using native share plugin");
        }
        #endif
    }
}