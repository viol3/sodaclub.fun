mergeInto(LibraryManager.library, 
{

  OpenURL : function(url)
  {
	url = UTF8ToString(url);
	window.open(url,'_blank');
  },
  
  GoogleLogin : function(clientId, nonce)
  {
	nonce = UTF8ToString(nonce);
	clientId = UTF8ToString(clientId);
	window.googleLogin(clientId, nonce);
  },


});