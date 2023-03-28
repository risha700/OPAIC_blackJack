using System.Globalization;

namespace BlackJack;
internal class Utils
{
    static int height = Console.WindowHeight;
    static int width = Console.WindowWidth;
    public static string Capitalize(string word){
        CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        TextInfo textInfo = cultureInfo.TextInfo;
        return textInfo.ToTitleCase(word);
    }
        public static void Spacer(char _='=', int count=10, bool empty=true){
        string str = new string(_, count);
        if (empty)
            str = new string(' ', count); 
        Console.WriteLine($"{str}");
        }
    public static string Stringify(string[] textArr, char delimeter = '\n'){
        return string.Join(delimeter, textArr);
    }
    public static dynamic Input(string message, bool required=false, bool stringType=false) {
        /*
            helper function to parse input to int32 or string
            method: safeParseInput
            params: string message
            returns: int or null
        */
        Console.CursorVisible = true;
        dynamic clean_input="";
        void handleInput(){
            try
            {
                Console.Write(message);
                if(stringType){
                    clean_input = Console.ReadLine();
                }else{
                    clean_input = Convert.ToInt32(Console.ReadLine());   
                }
                
            }
            catch (Exception e)
            {
                Write($"ERROR: {e.Message}", ConsoleColor.Red);
                // Console.Error.Write($"{e.Message}");
                if(required){
                    handleInput();
                }
            }
            if(String.IsNullOrEmpty(clean_input?.ToString()) && required){
                handleInput();
            }

        }
        // Console.CursorVisible = false;

        handleInput();
        return clean_input;
    }

    public static void Write(string text , ConsoleColor color=ConsoleColor.White, ConsoleColor bgColor=default, int? x_axis=null, int? y_axis=null)
    {
        /*
            helper wrapper around Console bacground and foreground
            method: writeInColor
            params: optional-> string text , ConsoleColor color, ConsoleColor bgColor
            returns: void (nil)
        */
        Console.CursorVisible = true;
        int? left = (x_axis is not null)? x_axis : Console.CursorLeft;
        int? top = (y_axis is not null)? y_axis : Console.CursorTop;

        Console.BackgroundColor = bgColor;
        Console.ForegroundColor = color;
        Console.SetCursorPosition((int) left, (int) top);
        Console.Write(text.ToString());
        Console.ResetColor();
    }

    public static void Render(string @string, int? x_axis=null, int? y_axis=null,
        bool renderSpace = false, bool erase=false, ConsoleColor? textColor=null, ConsoleColor? bgColor=null)
    {
        Console.CursorVisible = false;
        int x;
        int y;
        // get position 
        if((x_axis is not null) || (y_axis is not null)){
            x = (int) x_axis;
            y = (int) y_axis;
            Console.SetCursorPosition(x, y);
        }else{
            x=Console.CursorLeft;
            y=Console.CursorTop;
        }
        if(textColor is not null){
            Console.ForegroundColor = (ConsoleColor) textColor;
        }
        if(bgColor is not null){
            Console.BackgroundColor = (ConsoleColor) bgColor;
        }
        foreach (char c in @string)
            if ((c is '\n')||(c is ','))
                // go down 1 px on Y axis
                Console.SetCursorPosition(x, ++y);
            // when it is in view
            else if (Console.CursorLeft < width - 1 && (c is not ' ' || renderSpace || erase))
                if(erase){
                    Console.Write(' ');
                }else{
                    Console.Write(c);
                    Thread.Sleep(1); // dirty animmation
                }
                
            // safe move cursor into view
            else if (Console.CursorLeft < width - 1 && Console.CursorTop < height - 1)
                Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
            Console.ResetColor();
    }

    public static void DrawRect(int rectWidth, int rectHeight, (int x, int y ) point)
    {
        
        // Console.SetCursorPosition(point, y_start);
        Utils.Render($"┌", x_axis:point.x, y_axis:point.y); 
        for (int i = 1; i <rectWidth+2; i++)
        {
            Utils.Render($"─");                
        }
        Utils.Render($"┐"); 

        for (int i = 1; (i <rectHeight-1); i++)
        {
            Utils.Render($"│", x_axis:point.x+rectWidth+2, y_axis:i+point.y);                
        }
        Utils.Render($"┘", x_axis:point.x+rectWidth+2, y_axis:point.y+rectHeight-2);
        for (int i = 1; i <rectWidth+2; i++)
        {
            Utils.Render($"─", x_axis:point.x+i, y_axis:point.y+rectHeight-2);                
        }
        Utils.Render($"└", x_axis:point.x, y_axis:point.y+rectHeight-2); 
        for (int i = 1; i <rectHeight-2; i++)
        {
            Utils.Render($"│", x_axis:point.x, y_axis:(point.y+rectHeight-2-i));                
        }
    
    }
}    
