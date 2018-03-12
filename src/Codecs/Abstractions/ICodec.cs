namespace Codecs.Abstractions
{
    public interface ICodec<T1, T2>
    {
        T2 Decode(T1 input);
        T2 Encode(T1 input);        
    }
}
