using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class Quiz
{
    private List<Question> questions = new List<Question>();
    private int score = 0;
    private string selectedFile = "questions.json"; // Файл по умолчанию

    public void ShowMenu()
    {
        int selectedIndex = 0;
        string[] menuItems = { "Начать игру", "Выбрать файл с вопросами", "Просмотреть правила", "Выход" };

        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("        JS Quiz Game          ");
            Console.WriteLine("==============================");

            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("> " + menuItems[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("  " + menuItems[i]);
                }
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % menuItems.Length;
            }
            else if (key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
            }

        } while (key != ConsoleKey.Enter);

        switch (selectedIndex)
        {
            case 0:
                LoadQuestions(selectedFile);
                Start();
                break;
            case 1:
                SelectQuestionFile();
                break;
            case 2:
                ShowRules();
                break;
            case 3:
                Console.WriteLine("Спасибо за игру! До скорой встречи.");
                Environment.Exit(0);
                break;
        }
    }

    public void SelectQuestionFile()
    {
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("  Выбор файла с вопросами ");
        Console.WriteLine("==============================");

        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Questions");

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("Ошибка: папка Questions не найдена.");
            Console.WriteLine("Создайте папку и добавьте JSON-файлы.");
            Console.ReadLine();
            ShowMenu();
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.json");

        if (files.Length == 0)
        {
            Console.WriteLine("Ошибка: JSON-файлы не найдены.");
            Console.WriteLine("Поместите файлы с вопросами в папку `Questions`.");
            Console.ReadLine();
            ShowMenu();
            return;
        }

        int selectedIndex = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("  Выбор файла с вопросами ");
            Console.WriteLine("==============================");

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]); // Показываем только имя файла

                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("> " + fileName);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("  " + fileName);
                }
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % files.Length;
            }
            else if (key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex - 1 + files.Length) % files.Length;
            }

        } while (key != ConsoleKey.Enter);

        selectedFile = files[selectedIndex]; // Сохраняем полный путь файла
        Console.WriteLine($"Выбран файл: {Path.GetFileNameWithoutExtension(selectedFile)}");
        Console.WriteLine("==============================");
        Console.WriteLine("Нажмите Enter, чтобы продолжить.");
        Console.ReadLine();

        ShowMenu();
    }


    private void LoadQuestions(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Ошибка: файл не найден.");
                return;
            }

            string json = File.ReadAllText(filePath);
            var loadedQuestions = JsonSerializer.Deserialize<List<Question>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (loadedQuestions != null && loadedQuestions.Count > 0)
            {
                questions = loadedQuestions;
            }
            else
            {
                Console.WriteLine("Ошибка: JSON-файл пуст или поврежден.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки JSON: {ex.Message}");
        }
    }

    public void ShowRules()
    {
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("           ПРАВИЛА            ");
        Console.WriteLine("==============================");
        Console.WriteLine("1. Выберите правильный вариант ответа.");
        Console.WriteLine("2. Вопросы бывают с вариантами или с вводом текста.");
        Console.WriteLine("3. За правильный ответ начисляются баллы.");
        Console.WriteLine("4. После неверного ответа показывается объяснение.");
        Console.WriteLine("5. Победитель — тот, кто наберет больше очков!");
        Console.WriteLine("==============================");
        Console.WriteLine("Нажмите Enter, чтобы вернуться в меню.");
        Console.ReadLine();
        ShowMenu();
    }

    public void Start()
    {
        Console.Clear();
        Console.WriteLine("Добро пожаловать в викторину по JavaScript!");

        if (questions == null || questions.Count == 0)
        {
            Console.WriteLine("Ошибка: вопросы не загружены. Проверьте JSON-файл.");
            return;
        }

        foreach (var question in questions)
        {
            if (question == null || string.IsNullOrWhiteSpace(question.Text))
            {
                Console.WriteLine("Ошибка: вопрос поврежден или отсутствует!");
                continue;
            }

            Console.WriteLine($"\n==============================");
            Console.WriteLine($" Вопрос {questions.IndexOf(question) + 1}/{questions.Count} ");
            Console.WriteLine("==============================");
            Console.WriteLine(question.Text);

            bool correctAnswerGiven = false;

            while (!correctAnswerGiven)
            {
                if (!question.IsOpenEnded)
                {
                    if (question.Options != null && question.Options.Length > 0)
                    {
                        for (int i = 0; i < question.Options.Length; i++)
                        {
                            Console.WriteLine($"{i + 1}. {question.Options[i]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: варианты ответа отсутствуют.");
                    }

                    Console.Write("Выберите номер ответа: ");
                    string input = Console.ReadLine();
                    int choice;

                    if (int.TryParse(input, out choice) && choice > 0 && choice <= question.Options.Length)
                    {
                        if (question.Options[choice - 1] == question.CorrectAnswer)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Верно!");
                            score++;
                            correctAnswerGiven = true;
                            Console.ResetColor();

                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Неверно!");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Правильный ответ: {question.CorrectAnswer}");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"Объяснение: {question.Explanation}");
                            Console.ResetColor();
                            Console.WriteLine("Попробуйте снова!\n");
                        }
                    }
                }
                else
                {
                    Console.Write("Введите ответ: ");
                    string answer = Console.ReadLine();

                    if (answer.Trim().Equals(question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Верно!");
                        score++;
                        correctAnswerGiven = true;
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Неверно!");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Правильный ответ: {question.CorrectAnswer}");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Объяснение: {question.Explanation}");
                        Console.ResetColor();
                        Console.WriteLine("Попробуйте снова!\n");
                    }
                }
            }
        }


        Console.WriteLine($"\nВикторина завершена! Ваш счет: {score}/{questions.Count}");
        Console.WriteLine("Нажмите Enter, чтобы вернуться в меню.");
        Console.ReadLine();
        ShowMenu();
    }
}
