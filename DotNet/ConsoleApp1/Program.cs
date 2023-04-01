// See https://aka.ms/new-console-template for more information

using ET;

AAA().Coroutine();

async ETTask AAA()
{
    Console.WriteLine("AAA");
    await ETTask.CompletedTask;
}