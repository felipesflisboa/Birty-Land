var MobileCheckPlugin = {
	IsWebGLMobile: function() {
		return UnityLoader.SystemInfo.mobile; // WebGLMemorySize in bytes
	},
};

mergeInto(LibraryManager.library, MobileCheckPlugin);