/// Copyright (c) 2021 [DarkCompet](https://github.com/darkcompet)
namespace Tool.Compet.Http {
	using System;
	using System.Collections.Generic;
	using Tool.Compet.JsonWithNewton;
	using UnityEngine.Networking;
	using Tool.Compet.Log;
	
#if HAS_UNITASK
	using Cysharp.Threading.Tasks;
#else
	using System.Threading.Tasks;
#endif

	/// This uses `UniTask` which adapts asyn/await mechanism to replace Unity's Coroutine which has some drawbacks as:
	/// - Callback-hell.
	/// - Does not return value.
	/// - Can’t use try-catch-finally because of the yield syntax. So error cannot be handled due to the constraints.
	/// - Must allocate the lambda.
	/// - Difficult to do cancellation process (coroutine only stop, not call dispose).
	/// - Impossible to control multiple coroutines (serial/parallel processing).
	///
	/// Note that, `UniTask` runs completely on Unity's PlayerLoop, doesn't use threads. It can run on WebGL, wasm, etc.
	public class DkHttp {
		public bool contentTypeAsJson = true;
	
		private readonly Dictionary<string, string> requestHeaders = new();
	
		public DkHttp SetRequestHeader(string name, string value) {
			this.requestHeaders.Add(name, value);
			return this;
		}
	
		// Caller should call it from main thread since
		// we call `UnityWebRequest` which is Unity api that be must called at main thread.
#if HAS_UNITASK
		public async UniTask<T> Get<T>(string url) where T : TheApiResponse {
#else
		public async Task<T> Get<T>(string url) where T : TheApiResponse {
#endif
			// Perform try/catch for process
			try {
				// Build (compile) setting which was set by caller.
				// This normally generate config for next request.
				this.Build();
	
				// Use `using` keyword for auto-close resource to avoid memory leak.
				using var request = UnityWebRequest.Get(url);
	
				// Perform try/catch for request
				try {
					// Set up request
					foreach (var pair in this.requestHeaders) {
						request.SetRequestHeader(pair.Key, pair.Value);
					}
	
					// Perform request
					await request.SendWebRequest();
	
					// Done request, if failed, we return with http-response-code.
					if (request.result != UnityWebRequest.Result.Success) {
						DkLogs.Warning(this, $"GET failed, url: {url}, responseCode: {request.responseCode}, error: {request.error}");
	
						return DkObjects.NewInstace<T>().AlsoDk(res => {
							res.code = (int)request.responseCode;
							res.message = request.error;
						});
					}
				}
				catch (Exception e) {
					DkLogs.Warning(this, $"Could not GET, url: {url}, error: {e.Message}");
	
					return DkObjects.NewInstace<T>().AlsoDk(res => {
						res.code = (int)request.responseCode;
						res.message = e.Message;
					});
				}
	
				// Now, we are going to decode response body in json.
				// If error occured, we return result with UNKNOWN (-1) api-code.
				// Otherwise return response body (already contain api-code) from server.
				return DkJsons.Json2Obj<T>(request.downloadHandler.text).AlsoDk(res => {
					if (res.code != ApiCode.OK) {
						DkLogs.Warning(this, $"Failed response body GET from url: {url}", res);
					}
				});
			}
			catch (Exception e) {
				DkLogs.Warning(this, $"Unknown error while GET, url: {url}, error: {e.Message}");
	
				return DkObjects.NewInstace<T>().AlsoDk(res => {
					res.code = ApiCode.UNKNOWN;
					res.message = e.Message;
				});
			}
		}

#if HAS_UNITASK
		public async UniTask<T> Post<T>(string url, object serializableBody) where T : ApiResponse {
#else
		public async Task<T> Post<T>(string url, object serializableBody) where T : ApiResponse {
#endif
			// Perform try/catch for process
			try {
				// Build (compile) setting which was set by caller.
				// This normally generate config for next request.
				this.Build();
	
				// Use `using` keyword for auto-close resource to avoid memory leak.
				using var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
	
				// Perform try/catch for request
				try {
					// Setup request
					foreach (var pair in this.requestHeaders) {
						request.SetRequestHeader(pair.Key, pair.Value);
					}
					var bodyRaw = System.Text.Encoding.UTF8.GetBytes(DkJsons.Obj2Json(serializableBody));
					request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
					request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
	
					// Perform request
					await request.SendWebRequest();
	
					// Done request, if failed, we return with http-response-code.
					if (request.result != UnityWebRequest.Result.Success) {
						DkLogs.Warning(this, $"POST failed, url: {url}, responseCode: {request.responseCode}, error: {request.error}");
	
						return DkObjects.NewInstace<T>().AlsoDk(res => {
							res.code = (int)request.responseCode;
							res.message = request.error;
						});
					}
				}
				catch (Exception e) {
					DkLogs.Warning(this, $"Could not POST, url: {url}, error: {e.Message}");
	
					return DkObjects.NewInstace<T>().AlsoDk(res => {
						res.code = (int)request.responseCode;
						res.message = e.Message;
					});
				}
	
				// Now, we are going to decode response body in json.
				// If error occured, we return result with UNKNOWN (-1) api-code.
				// Otherwise return response body (already contain api-code) from server.
				return DkJsons.Json2Obj<T>(request.downloadHandler.text).AlsoDk(res => {
					if (res.code != ApiCode.OK) {
						DkLogs.Warning(this, $"Failed response body POST from url: {url}", res);
					}
				});
			}
			catch (Exception e) {
				DkLogs.Warning(this, $"Unknown error while POST, url: {url}, error: {e.Message}");
	
				return DkObjects.NewInstace<T>().AlsoDk(res => {
					res.code = ApiCode.UNKNOWN;
					res.message = e.Message;
				});
			}
		}
	
		private void Build() {
			if (this.contentTypeAsJson) {
				this.requestHeaders.Add("Content-Type", "application/json");
			}
		}
	}
}
