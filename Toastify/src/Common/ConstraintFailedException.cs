using System;
using System.Linq.Expressions;

namespace Toastify.Common
{
    public class ConstraintFailedException : ApplicationException
    {
        public ConstraintFailedException(Expression constraint) : base($"Constraint failed:\n    {constraint}\n")
        {
        }
    }
}