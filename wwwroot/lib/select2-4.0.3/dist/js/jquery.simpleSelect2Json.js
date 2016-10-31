(function ($) {
	$.fn.simpleSelect2Json = function (src,key,text) {
		var obj = this;
		var URLPattern = /((\w+:\/\/)[-a-zA-Z0-9:@;?&=\/%\+\.\*!'\(\),\$_\{\}\^~\[\]`#|]+)/g;
		var lang = "pt-BR"
		
		$.fn.select2.defaults.set('amdBase', 'select2/');
		$.fn.select2.defaults.set('amdLanguageBase', 'select2/i18n/');
		
		if(URLPattern.test(src)) {
		
			this.select2({
				language: lang,
				placeholder:"Buscando..."
			});
		
			this.prop('disabled',true);

			$.get(src, function( data ) {
				if((typeof data)==="string") {
					try{data = $.parseJSON(data);}
					catch(e){console.error("Cannot parse the data from server ");return;}
				}
				obj.append(renderOptionList(data,key,text));
				obj.prop('disabled',false);
			});
			
		} 
		else {
			this.select2({
				language : lang
			});
		
			this.prop('disabled',true);

			try {
				var list = $.parseJSON(src);
				obj.append(renderOptionList(list,key,text));
				this.prop('disabled',false); 
			} 
			catch(e) {
				console.error('Invalid JSON object!!');
			}
		}

		function renderOptionList (list,key,text) {
			var result = "";
			
			if(list instanceof Array){
				list.forEach(function(currentValue, index,arr){
					if(key in currentValue && text in currentValue){
						result = result.concat("<option value='"+currentValue[key]+"'>"+currentValue[text]+'</option>');
					} 
					else {
						console.error('The JSON object has not that propeties!');
					}
				});
			}
			else {
				console.error('JSON object is not an Array!');
			}
			return result;
		}
	}
}(jQuery));
