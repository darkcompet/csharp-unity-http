// Comment since unused

// using System;
// using System.Runtime.CompilerServices;
// using UnityEngine;
// using UnityEngine.Networking;

// /// Ref: https://qiita.com/satotin/items/579fa3b9da0ad0d899e8

// namespace Tool.Compet.HttpWithUniTask.Internal {
	// public class UnityWebRequestAwaiterExt : INotifyCompletion {
	// 	private UnityWebRequestAsyncOperation asyncOperation;
	// 	private Action continuation;

	// 	public UnityWebRequestAwaiterExt(UnityWebRequestAsyncOperation asyncOperation) {
	// 		this.asyncOperation = asyncOperation;

	// 		// Register completed-listener to the asyncOperation
	// 		asyncOperation.completed += OnRequestCompleted;
	// 	}

	// 	// Called by awaiter
	// 	public bool IsCompleted {
	// 		get {
	// 			return this.asyncOperation.isDone;
	// 		}
	// 	}

	// 	// Called by awaiter
	// 	public void GetResult() { }

	// 	// Called by awaiter
	// 	public void OnCompleted(Action continuation) {
	// 		this.continuation = continuation;
	// 	}

	// 	private void OnRequestCompleted(AsyncOperation asyncOperation) {
	// 		this.continuation.Invoke();
	// 	}
	// }

	// public static class ExtensionMethods {
	// 	public static MyUnityWebRequestAwaiterExt GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation) {
	// 		return new MyUnityWebRequestAwaiterExt(asyncOperation);
	// 	}
	// }
// }
