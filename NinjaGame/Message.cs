using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Message
{
    public string Content { get; private set; }

    public Message(string content)
    {
        Content = content;
    }

    public void Display()
    {
        Console.WriteLine($"{Content}");
    }
}
