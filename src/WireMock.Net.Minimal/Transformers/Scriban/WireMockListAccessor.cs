// Copyright © WireMock.Net

using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace WireMock.Transformers.Scriban
{
    internal class WireMockListAccessor : IListAccessor, IObjectAccessor
    {
        #region IListAccessor
        public int GetLength(TemplateContext context, SourceSpan span, object target)
        {
            throw new NotImplementedException();
        }

        public object GetValue(TemplateContext context, SourceSpan span, object target, int index)
        {
            return target.ToString();
        }

        public void SetValue(TemplateContext context, SourceSpan span, object target, int index, object value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IObjectAccessor
        public int GetMemberCount(TemplateContext context, SourceSpan span, object target)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetMembers(TemplateContext context, SourceSpan span, object target)
        {
            throw new NotImplementedException();
        }

        public bool HasMember(TemplateContext context, SourceSpan span, object target, string member)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TemplateContext context, SourceSpan span, object target, string member, out object value)
        {
            throw new NotImplementedException();
        }

        public bool TrySetValue(TemplateContext context, SourceSpan span, object target, string member, object value)
        {
            throw new NotImplementedException();
        }

        public bool TryGetItem(TemplateContext context, SourceSpan span, object target, object index, out object value)
        {
            throw new NotImplementedException();
        }

        public bool TrySetItem(TemplateContext context, SourceSpan span, object target, object index, object value)
        {
            throw new NotImplementedException();
        }

        public bool HasIndexer => throw new NotImplementedException();

        public Type IndexType => throw new NotImplementedException();
        #endregion
    }
}