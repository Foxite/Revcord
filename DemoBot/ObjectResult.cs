using Qmmands;

namespace DemoBot; 

public class ObjectResult : CommandResult {
	public override bool IsSuccessful => true;
	
	public object Object { get; set; }
	
	public ObjectResult(object @object) {
		Object = @object;
	}
}
