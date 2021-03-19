using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public class SignalrHub : Hub
    {
        public void test(String text)
        {
            Console.WriteLine(text);
        }
    }
}
