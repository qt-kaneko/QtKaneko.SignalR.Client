namespace QtKaneko.SignalR.Client.Extensions;

[Flags]
public enum ArraySegmentSplitOptions
{
  None,
  RemoveEmptyEntries
}
public struct ArraySegmentEntryEnumerator<T>
{
  public ArraySegment<T> Current { get; private set; }

  readonly ArraySegment<T> _arraySegment;
  readonly T _separator;
  readonly ArraySegmentSplitOptions _options;

  int _i;

  public ArraySegmentEntryEnumerator(in ArraySegment<T> arraySegment, in T separator,
                                     ArraySegmentSplitOptions options)
  {
    _arraySegment = arraySegment;
    _separator = separator;
    _options = options;
  }

  public bool MoveNext()
  {
    if (_i > _arraySegment.Count) return false;

    var length = 0;

    for (int i = _i; i < _arraySegment.Count; ++i)
    {
      if (!EqualityComparer<T>.Default.Equals(_arraySegment[i], _separator)) continue;
      
      length = i - _i;
      if (_options.HasFlag(ArraySegmentSplitOptions.RemoveEmptyEntries) && length == 0) continue;

      Current = new ArraySegment<T>(_arraySegment.Array!, _arraySegment.Offset + _i, length);
      _i += length + 1;
      return true;
    }

    length = _arraySegment.Count - _i;
    if (_options.HasFlag(ArraySegmentSplitOptions.RemoveEmptyEntries) && length == 0) return false;

    Current = new ArraySegment<T>(_arraySegment.Array!, _arraySegment.Offset + _i, length);
    _i += length + 1;
    return true;
  }

  public ArraySegmentEntryEnumerator<T> GetEnumerator() => this;
}
static class ArraySegmentExtensions
{
  public static ArraySegmentEntryEnumerator<T> Split<T>(
    this in ArraySegment<T> @this,
    in T separator, ArraySegmentSplitOptions options = ArraySegmentSplitOptions.None)
  {
    return new ArraySegmentEntryEnumerator<T>(@this, separator, options);
  }
}