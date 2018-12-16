
mergeInto(LibraryManager.library, {
    WindowAlert : function(message)
    {
        window.alert(Pointer_stringify(message));
    },
    DownloadFile : function(array, size, fileNamePtr)
    {
        var fileName = Pointer_stringify(fileNamePtr);
     
        var bytes = new Uint8Array(size);
        for (var i = 0; i < size; i++)
        {
           bytes[i] = HEAPU8[array + i];
        }
     
        var blob = new Blob([bytes]);
        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
     
        var event = document.createEvent("MouseEvents");
        event.initMouseEvent("click", true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
        link.dispatchEvent(event);
    },

	
	LoadData : function()
	{
		var input = document.createElement('input');
		input.setAttribute('type', 'file');
		input.onclick = function (event) {
        this.value = null;
		};
		input.click();
		input.onchange = function (event) {
			SendMessage('GlobalController', 'FileSelected', URL.createObjectURL(event.target.files[0]));
			input.parentNode.removeChild(element);
		}

    }
});




