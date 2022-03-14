using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;
using Firebase.Unity;
using Proyecto26;
using System.Threading.Tasks;
using System.Threading;

using Newtonsoft.Json;

using System.Net;
using static System.IO.Path;



public class uploadarte : MonoBehaviour
{

	//public Sprite j;
		public Image im;
		public spawnart spw;
		public Sprite h;
		public static string name1;

		    [Header("Firebase")]
    public FirebaseStorage storage;
    public StorageReference storageReference;
    public DatabaseReference reference;
	

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void PostToDatabase(string name)
    {
		//DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		//var childReference = reference.Child("digitalart");
		//var key = childReference.Key;

		//DatabaseReference fireRef = new Firebase("https://fir-artscape-default-rtdb.firebaseio.com/digitalart");
		//var newUserRef = fireRef.Push();
		//childReference.Child(key).SetValueAsync(name);



		myirt user = new myirt();
        RestClient.Put("https://fir-artscape-default-rtdb.firebaseio.com/digitalart/"+ name + ".json", user);
    }

	public void pressedup()
    {
        PickImage( 512 );
		

    }

    private void PickImage( int maxSize )
{
	
	storage = FirebaseStorage.DefaultInstance;
    storageReference = storage.GetReferenceFromUrl("gs://fir-artscape.appspot.com/digitalart");
	

	NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
	{
		Debug.Log( "Image path: " + path );
		if( path != null )
		{
			string localFile = path;
			string name = GetFileNameWithoutExtension(localFile);
			string extension = GetExtension(localFile);

			StorageReference riversRef = storageReference.Child(name + extension);


			// Create Texture from selected image
			Texture2D texture = NativeGallery.LoadImageAtPath( path, maxSize );
			
			if( texture == null )
			{
				Debug.Log( "Couldn't load texture from " + path );
				return;
			}
			
			var newMetadata = new MetadataChange();
					newMetadata.ContentType = "image/" + extension.Substring(1);


			riversRef.PutFileAsync(localFile, newMetadata, null, CancellationToken.None).ContinueWithOnMainThread((Task<StorageMetadata> task) => {
				if (task.IsFaulted || task.IsCanceled) {
					Debug.Log(task.Exception.ToString());
					// Uh-oh, an error occurred!
				}
				else {
					// Metadata contains file metadata such as size, content-type, and download URL.
					StorageMetadata metadata = task.Result;
					
					string md5Hash = metadata.Md5Hash;

					



					Debug.Log("Finished uploading...");
					Debug.Log("md5 hash = " + md5Hash);
					name1=name;
					PostToDatabase(name);
				}
			});
			Rect hi = new Rect(0, 0, texture.width, texture.height);
			Vector2 jk = new Vector2(0.5f, 0.5f);

			h = Sprite.Create(texture, hi, jk);
			im.GetComponent<Image>().sprite = h;
			//spw.pressi(h);

			/*
			// Assign texture to a temporary quad and destroy it after 5 seconds
			GameObject quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
			

			quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
			quad.transform.forward = Camera.main.transform.forward;
			quad.transform.localScale = new Vector3( 1f, texture.height / (float) texture.width, 1f );

			Material material1 = quad.GetComponent<Renderer>().material;
			if( !material1.shader.isSupported ) // happens when Standard shader is not included in the build
				material1.shader = Shader.Find( "Legacy Shaders/Diffuse" );

			material1.mainTexture = texture;
			*/

			

			

			//Destroy( quad, 5f );

			// If a procedural texture is not destroyed manually, 
			// it will only be freed after a scene change
			//Destroy( texture, 5f );
		}
	} );

	Debug.Log( "Permission result: " + permission );
}
}
