namespace Revcord;

public delegate Task AsyncEventHandler<in TSender, in TArgs>(TSender sender, TArgs args);
