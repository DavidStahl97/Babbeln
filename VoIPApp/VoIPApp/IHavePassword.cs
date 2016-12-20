using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp
{
    public interface IHavePassword
    {
        SecureString Password { get; }
        SecureString ConfirmationPassword { get; }
    }
}
