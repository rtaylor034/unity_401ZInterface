using System;
using System.Collections;
using System.Collections.Generic;
using MorseCode.ITask;
using UnityEngine;
using GStructures;
using System.Threading.Tasks;
namespace Expressions
{
    namespace Reference
    {
        using Identifier;
        namespace Identifier
        {
            public interface IIdentifier { }
            public sealed class Defined : IIdentifier
            {
                private readonly Type _tokenType;
                public Defined(Type tokenType)
                {
                    _tokenType = tokenType;
                }
                public override bool Equals(object obj) => obj is Defined other && _tokenType.Equals(other._tokenType);
                public override int GetHashCode() => _tokenType.GetHashCode();
                public override string ToString() => $"DEFINED-{_tokenType.Name}";
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
                public override string ToString() => $"DYNAMIC-'{_label}'";
            }
        }
        public interface IProvider
        {
            public Referable GetReference(IIdentifier key);
            public sealed class None : IProvider
            {
                public Referable GetReference(IIdentifier key) => throw new Exception($"IReference Provider supplied with undefined key: <{key}>");
            }
        }
        public class Scope : IProvider
        {
            private IProvider _parent;
            private Dictionary<IIdentifier, Referable> _map;
            public Scope(IProvider parent, params (IIdentifier id, Referable refToken)[] references)
            {
                _parent = parent;
                // i KNOW there is an idiomatic way to do this that isnt retarded.
                _map = new(references.Map(x => KeyValuePair.Create(x.id, x.refToken)));
            }
            public Scope(params (IIdentifier id, Referable refToken)[] references) : this(new IProvider.None(), references) { }
            public Referable GetReference(IIdentifier key)
            {
                return _map.TryGetValue(key, out Referable referable)
                    ? referable
                    : _parent.GetReference(key);
            }
        }
        
        public class Referable
        {
            private Token.IToken<object> _evalToken;
            private object _thisResolution;
            private bool _resolved = false;

            private Referable(Token.IToken<object> token) => _evalToken = token;
            public ITask<object> Resolve(Token.ResolutionContext scope)
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
            public static Referable Create(Token.IToken<object> token) => new(token);
        }
    }
    
    public class Expression<T>
    {
        public Reference.Scope References;
        public Token.IToken<T> ResolutionToken;

        public Expression(Reference.Scope references, Token.IToken<T> resolutionToken)
        {
            References = references;
            ResolutionToken = resolutionToken;
        }
        public ITask<T> Resolve(Token.IResolver resolver) => ResolutionToken.Resolve(new Token.ResolutionContext { Resolver = resolver, Scope = References });
    }
    
    
}