using System;
using System.Collections;
using System.Collections.Generic;
using MorseCode.ITask;
using UnityEngine;
using GStructures;
using System.Threading.Tasks;
namespace Expressions
{
    namespace References
    {
        using Identifier;
        namespace Identifier
        {
            public interface IIdentifier { }
            public sealed class Contextual : IIdentifier
            {
                private readonly Type _tokenType;
                public Contextual(Type tokenType)
                {
                    _tokenType = tokenType;
                }
                public override bool Equals(object obj) => obj is Contextual other && _tokenType.Equals(other._tokenType);
                public override int GetHashCode() => _tokenType.GetHashCode();
            }
            public sealed class Dynamic : IIdentifier
            {
                private readonly string _label;
                public Dynamic(string label)
                {
                    _label = label;
                }
                public override bool Equals(object obj) => obj is Dynamic other && _label.Equals(other._label);
                public override int GetHashCode() => _label.GetHashCode();
            }
        }
        public interface IProvider
        {
            public Option<Referable> GetReference(IIdentifier key);
            public sealed class None : IProvider
            {
                public Option<Referable> GetReference(IIdentifier key) => throw new Exception($"IReference Provider supplied with undefined key: {key}");
            }
        }
        public class Map : IProvider
        {
            private IProvider _parent;
            private Dictionary<IIdentifier, Referable> _map;
            public Map(IProvider parent, Dictionary<IIdentifier, Referable> map)
            {
                _parent = parent;
                _map = map;
            }
            public Option<Referable> GetReference(IIdentifier key)
            {
                return _map.TryGetValue(key, out Referable referable)
                    ? new Option<Referable>.Some(referable)
                    : _parent.GetReference(key);
            }
        }
        public abstract class Token<T> : Token.IToken<T>
        {
            public IIdentifier Identifier;
            protected Token(IIdentifier identifier) => Identifier = identifier;
            public ITask<T> Resolve(IProvider scope)
            {

            }
        }
        public class Referable
        {
            private Token.IToken<object> _evalToken;
            private object _thisResolution;
            private bool _resolved = false;

            public Referable(Token.IToken<object> token) => _evalToken = token;
            public ITask<object> Resolve(IProvider scope)
            {
                return _resolved switch
                {
                    true => Task.FromResult(_thisResolution).AsITask(),
                    false => _evalToken.Resolve(scope)
                };
            }
            public void Reset()
            {
                if (!_resolved) throw new System.Exception("Referable Reset() before being evaluated");
                _resolved = false;
            }
        }
    }
    
    public abstract class Expression<T>
    {
        public References.Map References;
        public Token.IToken<T> ResolutionToken;

        protected Expression(References.Map references, Token.IToken<T> resolutionToken)
        {
            References = references;
            ResolutionToken = resolutionToken;
        }
        public ITask<T> Resolve()
        {

        }
    }
    
    
}