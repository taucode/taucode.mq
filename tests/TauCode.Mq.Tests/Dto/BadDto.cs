using System;

namespace TauCode.Mq.Tests.Dto
{
    /// <summary>
    /// Serialization exception test DTO
    /// </summary>
    public class BadDto
    {
        public string Name
        {
            get => throw new NotSupportedException();
            set
            {
                // idle
            }
        }
    }
}
