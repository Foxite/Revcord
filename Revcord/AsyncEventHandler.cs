namespace Revcord;

public delegate Task AsyncEventHandler<in TArgs>(TArgs args) where TArgs : AsyncEventArgs;
