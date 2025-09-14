mergeInto(LibraryManager.library, 
{

  OpenURL : function(url)
  {
	url = UTF8ToString(url);
	window.open(url,'_blank');
  },

});