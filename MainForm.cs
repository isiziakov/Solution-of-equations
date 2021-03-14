using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Solution_of_equations
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        //объявление переменных
        char variables;
        int index, current_degree, character_index, character_number, character, degree, number, number_pos, multiplier_degree, max_degree, max_negative_degree, first_bracket, second_bracket, bracket_depth;
        double x_n, n1, n2, current_coefficient, multiplier;
        string equation, line_of_output;
        double[] coefficients = new double[10];
        double[] negative_coefficients = new double[10];

        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        // вызов справки через меню
        void СправкаToolStripMenuItemClick(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
        }
        // завершение работы программы при нажатии кнопки "выход" в меню
        void ВыходToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }
        // вызов обратной связи через меню
        void ОбратнаяСвязьToolStripMenuItemClick(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }
        // нажатие на кнопку "Новое уравнение"
        void Button2Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox1.Clear();
        }
        // нажатие на кнопку "Новое уравнение" в меню
        void НовоеУравнениToolStripMenuItemClick(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox1.Clear();
        }
        // нажатие на кнопку "Решить"
        void Button1Click(object sender, EventArgs e)
        {
            try
            {
                // обнуление переменных
                zeroing_variables();
                // очистка строки вывода решений
                textBox2.Clear();
                // ввод строки
                equation = textBox1.Text;
                equation = equation.ToLower();
                // замена произвольной буквенной переменной на x
                variable_definition();
                // удаление промежутков между символами
                equation = equation.Replace(" ", "");
                // замена х^1 на х
                equation = equation.Replace("x^1", "x");
                // возведение чисела в степень
                the_construction_of_the_numbers_in_the_degree();
                // проверка введенной строки
                if (checking_the_entered_line() == true)
                {
                    return;
                }
                textBox2.AppendText("Введено уравнение: " + equation);
                // выполнение действий со скобками
                executing_actions_with_brackets();
                equation = equation.Replace("x^1", "x");
                // нахождение коэффициентов в уравнении
                solution_of_equation();
                line_of_output = "";
                // определение максимальной степени в уравнении
                for (degree = 9; degree > 0; degree--)
                {
                    if (coefficients[degree] != 0)
                    {
                        max_degree = degree;
                        break;
                    }
                }
                // определение максимальной отрицательной степени в уравнении
                for (degree = 9; degree > 0; degree--)
                {
                    if (negative_coefficients[degree] != 0)
                    {
                        max_negative_degree = degree;
                        break;
                    }
                }
                // если в уравнении есть переменная в отрицательной степени
                if (max_negative_degree > 0)
                {
                    // если после умножения уравнения на переменную в максимальной отрицательной степени степень переменной > 9
                    if (max_negative_degree + max_degree > 9)
                    {
                        throw new Exception("big degree");
                    }
                    // умножение уравнения на переменную в максимальной отрицательной степени
                    for (degree = max_degree; degree > -1; degree--)
                    {
                        coefficients[degree + max_negative_degree] = coefficients[degree];
                        coefficients[degree] = 0;
                    }
                    for (degree = max_negative_degree; degree > 0; degree--)
                    {
                        coefficients[max_negative_degree - degree] = negative_coefficients[degree];
                    }
                }
                textBox2.AppendText("\r\n" + "Степень уравнения равна " + max_degree);
                textBox2.AppendText("\r\n" + "Коэффициенты уравнения:");
                // обновление значения максимальной степени
                max_degree = max_degree + max_negative_degree;
                // приведение уравнения к виду n*k^9 + ... m*k^2 + b*x + k = 0
                for (degree = max_degree; degree > 0; degree--)
                {
                    textBox2.AppendText("\r\n" + "При x^" + degree + " = " + coefficients[degree] + ";");
                    line_of_output += coefficients[degree] + "x";
                    if (degree > 1)
                    {
                        line_of_output = line_of_output + "^" + degree;
                    }
                    if (coefficients[degree - 1] >= 0)
                    {
                        line_of_output += " + ";
                    }
                }
                degree = max_degree;
                textBox2.AppendText("\r\n" + "Свободный член = " + coefficients[0] + ".");
                line_of_output += coefficients[0] + " = 0";
                textBox2.AppendText("\r\n" + "Уравнение можно привести к виду: " + line_of_output);
                // если после приведения все переменные сокращаются
                if (max_degree == 0)
                {
                    // если свободный член = 0
                    if (coefficients[0] == 0)
                    {
                        textBox2.AppendText("\r\n" + "Решением уравнения является любое число");
                        textBox2.AppendText("\r\n" + "Ответ: ∞");
                    }
                    else
                    {
                        MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox2.AppendText("\r\n" + "Причина ошибки - после вычисления коэффициентов при переменных все переменные сокращаются, и уравнение принимает вид \"k = 0\"");
                    }
                    return;
                }
                // если уравнение - линейное
                if (max_degree == 1)
                {
                    textBox2.AppendText("\r\n" + "Решаем линейное уравнение:");
                    solution_of_equations_of_the_first_degree();
                    line_of_output = "";
                    return;
                }
                // если уравнение - квадратное
                if (max_degree == 2)
                {
                    textBox2.AppendText("\r\n" + "Решаем через дискриминант:");
                    solution_to_the_quadratic_equation();
                    line_of_output = "";
                    return;
                }
                // если уравнение - кубическое
                if (max_degree == 3)
                {
                    textBox2.AppendText("\r\n" + "Решаем по тригонометрической формуле Виета:");
                    solving_cubic_equations();
                    line_of_output = "";
                    return;
                }
                // если степень уравнения > 3
                if (max_degree > 3)
                {
                    textBox2.AppendText("\r\n" + "Решаем по теореме Горнера");
                    solution_on_Gorner_theorem();
                    line_of_output = "";
                    return;
                }
            }
            catch (Exception exc)
            {
                if (exc.Message == "big degree")
                {
                    MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.AppendText("Причина ошибки - после умножения уравнения на переменную в максимальной отрицательной степени оно содержит переменную в степени больше 9");
                    return;
                }
                if (exc.Message == "big degree two")
                {
                    MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.AppendText("Максимальная степень переменной во введенном уравнении больше 9");
                    return;
                }
                if (exc.Message == "unacceptable sign in brakcets")
                {
                    MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.AppendText("Причина ошибки - внутри скобок стоит недопустимый символ - '='");
                    return;
                }
                if (exc.Message == "unpared brakcets")
                {
                    MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.AppendText("Причина ошибки - в уравнении присутствуют непарные скобки");
                    return;
                }
                if (exc.Message == "wrong degree")
                {
                    MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.AppendText("Причина ошибки - недопустимая степень");
                    return;
                }
                if (exc.Message == "wrong degree two")
                {
                    MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.AppendText("Причина ошибки - степень не является целым числом");
                    return;
                }
                if (exc.Message == "division is impossible")
                {
                    MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.AppendText("Причина ошибки - невозможность выполнить деление многочленов, т.к. после деления остается остаток");
                    return;
                }
            }
        }
        // обработка строки
        public void solution_of_equation()
        {
            // нахождение коэффициента при x^9
            current_degree = 9;
            analysis_of_the_row("x^9");
            // нахождение коэффициента при x^8
            current_degree = 8;
            analysis_of_the_row("x^8");
            // нахождение коэффициента при x^7
            current_degree = 7;
            analysis_of_the_row("x^7");
            // нахождение коэффициента при x^6
            current_degree = 6;
            analysis_of_the_row("x^6");
            // нахождение коэффициента при x^5
            current_degree = 5;
            analysis_of_the_row("x^5");
            // нахождение коэффициента при x^4
            current_degree = 4;
            analysis_of_the_row("x^4");
            // нахождение коэффициента при x^3
            current_degree = 3;
            analysis_of_the_row("x^3");
            // нахождение коэффициента при x^2
            current_degree = 2;
            analysis_of_the_row("x^2");
            // нахождение коэффициента при x
            current_degree = 1;
            analysis_of_the_row("x");
            // нахождение свободного члена
            coefficients[0] += analysis_of_the_row_without_coefficients_x(equation);
        }
        // выполнение действий со скобками
        public void executing_actions_with_brackets()
        {
            while (equation.IndexOf('(') != -1)
            {
                first_bracket = equation.IndexOf('(');
                determining_the_depth_of_brackets();
                if (first_bracket > 0 && equation[first_bracket - 1] == '-')
                {
                    sign_change_in_brakets(first_bracket, second_bracket);
                }
                if (first_bracket > 0 && equation[first_bracket - 1] == '*')
                {
                    performing_multiplication_before_brakets();
                }
                if (first_bracket > 0 && equation[first_bracket - 1] == '/')
                {
                    performing_multiplication_before_brakets();
                }
                if (second_bracket < equation.Length - 1 && equation[second_bracket + 1] == '^')
                {
                    construction_of_an_expression_in_degree();
                }
                while (second_bracket < equation.Length - 2 && (equation[second_bracket + 1] == '*' || equation[second_bracket + 1] == '/') && equation[second_bracket + 2] == '(')
                {
                    multiplication_brackets();
                }
                if (second_bracket < equation.Length - 1 && (equation[second_bracket + 1] == '*' || equation[second_bracket + 1] == '/'))
                {
                    performing_multiplication_after_brakets();
                }
                equation = equation.Remove(second_bracket, 1);
                equation = equation.Remove(first_bracket, 1);
            }
        }

        public void determining_the_depth_of_brackets()
        {
            for (int n = first_bracket + 1; n < equation.Length; n++)
            {
                if (equation[n] == '(')
                {
                    first_bracket = n;
                    determining_the_depth_of_brackets();
                }
                if (equation[n] == ')')
                {
                    second_bracket = n;
                    return;
                }
                if (equation[n] == '=')
                {
                    throw new Exception("unacceptable sign in brakcets");
                }
                if (n == equation.Length - 1)
                {
                    throw new Exception("unpared brakcets");
                }
            }
        }

        public void multiplication_brackets()
        {
            int first_bracket_1 = first_bracket;
            int second_bracket_1 = second_bracket;
            int n = 1;
            for (degree = second_bracket + 3; degree < equation.Length; degree++)
            {
                if (equation[degree] == '(')
                {
                    n = n + 1;
                }
                if (equation[degree] == ')')
                {
                    n = n - 1;
                }
                if (n == 0)
                {
                    break;
                }
                if (equation[degree] == '=')
                {
                    throw new Exception("unacceptable sign in brakcets");
                }
                if (degree == equation.Length - 1)
                {
                    throw new Exception("unpared brakcets");
                }
            }
            int first_bracket_2 = second_bracket + 2;
            int second_bracket_2 = degree;
            string equation_two = equation;
            equation = equation.Substring(first_bracket_2 + 1, second_bracket_2 - first_bracket_2 - 1) + "=";
            executing_actions_with_brackets();
            solution_of_equation();
            double[] second_coefficients = new double[10];
            double[] second_negative_coefficients = new double[10];
            int max_degree_1 = 0, max_negative_degree_1 = 0, max_degree_2 = 0, max_negative_degree_2 = 0;
            for (degree = 9; degree > -1; degree--)
            {
                if (coefficients[degree] != 0 && max_degree_2 == 0)
                {
                    max_degree_2 = degree;
                }
                if (negative_coefficients[degree] != 0 && max_negative_degree_2 == 0)
                {
                    max_negative_degree_2 = degree;
                }
                second_coefficients[degree] = coefficients[degree];
                second_negative_coefficients[degree] = negative_coefficients[degree];
                coefficients[degree] = 0;
                negative_coefficients[degree] = 0;
            }
            equation = equation_two.Substring(first_bracket_1 + 1, second_bracket_1 - first_bracket_1 - 1) + "=";
            solution_of_equation();
            double[] first_coefficients = new double[10];
            double[] first_negative_coefficients = new double[10];
            for (degree = 9; degree > -1; degree--)
            {
                if (coefficients[degree] != 0 && max_degree_1 == 0)
                {
                    max_degree_1 = degree;
                }
                if (negative_coefficients[degree] != 0 && max_negative_degree_1 == 0)
                {
                    max_negative_degree_1 = degree;
                }
                first_coefficients[degree] = coefficients[degree];
                first_negative_coefficients[degree] = negative_coefficients[degree];
                coefficients[degree] = 0;
                negative_coefficients[degree] = 0;
            }
            if (max_degree_1 + max_degree_2 > 9 || max_negative_degree_1 + max_negative_degree_2 > 9)
            {
                throw new Exception("big degree two");
            }
            line_of_output = "";
            int two_degree;
            if (equation_two[second_bracket_1 + 1] == '*')
            {
                for (degree = max_degree_1; degree > -1; degree--)
                {
                    for (two_degree = max_degree_2; two_degree > -1; two_degree--)
                    {
                        if (first_coefficients[degree] != 0 && second_coefficients[two_degree] != 0)
                        {
                            if (degree + two_degree > 1)
                            {
                                line_of_output = line_of_output + first_coefficients[degree] * second_coefficients[two_degree] + "x^" + (degree + two_degree) + "+";
                            }
                            if (degree + two_degree == 1)
                            {
                                line_of_output = line_of_output + first_coefficients[degree] * second_coefficients[two_degree] + "x+";
                            }
                            if (degree + two_degree == 0)
                            {
                                line_of_output = line_of_output + first_coefficients[degree] * second_coefficients[two_degree] + "+";
                            }
                        }
                    }
                    for (two_degree = max_negative_degree_2; two_degree > 0; two_degree--)
                    {
                        if (degree - two_degree == 0 && first_coefficients[degree] != 0 && second_negative_coefficients[two_degree] != 0)
                        {
                            line_of_output = line_of_output + first_coefficients[degree] * second_negative_coefficients[two_degree] + "+";
                        }
                        if (degree - two_degree > 0 && first_coefficients[degree] != 0 && second_negative_coefficients[two_degree] != 0)
                        {
                            if (degree - two_degree > 1)
                            {
                                line_of_output = line_of_output + first_coefficients[degree] * second_negative_coefficients[two_degree] + "x^" + (degree - two_degree) + "+";
                            }
                            if (degree - two_degree == 1)
                            {
                                line_of_output = line_of_output + first_coefficients[degree] * second_negative_coefficients[two_degree] + "x+";
                            }
                        }
                        if (degree - two_degree < 0 && first_coefficients[degree] != 0 && second_negative_coefficients[two_degree] != 0)
                        {
                            if (degree - two_degree < -1)
                            {
                                line_of_output = line_of_output + first_coefficients[degree] * second_negative_coefficients[two_degree] + "/x^" + Math.Abs(degree - two_degree) + "+";
                            }
                            if (degree - two_degree == -1)
                            {
                                line_of_output = line_of_output + first_coefficients[degree] * second_negative_coefficients[two_degree] + "/x+";
                            }
                        }
                    }
                }
                for (degree = max_negative_degree_1; degree > 0; degree--)
                {
                    for (two_degree = max_negative_degree_2; two_degree > 0; two_degree--)
                    {
                        if (first_negative_coefficients[degree] != 0 && second_negative_coefficients[two_degree] != 0)
                        {
                            if (degree + two_degree > 1)
                            {
                                line_of_output = line_of_output + first_negative_coefficients[degree] * second_negative_coefficients[two_degree] + "/x^" + (degree + two_degree) + "+";
                            }
                            if (degree + two_degree == 1)
                            {
                                line_of_output = line_of_output + first_negative_coefficients[degree] * second_negative_coefficients[two_degree] + "/x+";
                            }
                        }
                    }
                    for (two_degree = max_degree_2; two_degree > -1; two_degree--)
                    {
                        if (two_degree - degree == 0 && first_negative_coefficients[degree] != 0 && second_coefficients[two_degree] != 0)
                        {
                            line_of_output = line_of_output + first_negative_coefficients[degree] * second_coefficients[two_degree] + "+";
                        }
                        if (two_degree - degree > 0 && first_negative_coefficients[degree] != 0 && second_coefficients[two_degree] != 0)
                        {
                            if (two_degree - degree > 1)
                            {
                                line_of_output = line_of_output + first_negative_coefficients[degree] * second_coefficients[two_degree] + "x^" + (two_degree - degree) + "+";
                            }
                            if (two_degree - degree == 1)
                            {
                                line_of_output = line_of_output + first_negative_coefficients[degree] * second_coefficients[two_degree] + "x+";
                            }
                        }
                        if (two_degree - degree < 0 && first_negative_coefficients[degree] != 0 && second_coefficients[two_degree] != 0)
                        {
                            if (two_degree - degree < -1)
                            {
                                line_of_output = line_of_output + first_negative_coefficients[degree] * second_coefficients[two_degree] + "/x^" + Math.Abs(two_degree - degree) + "+";
                            }
                            if (two_degree - degree == -1)
                            {
                                line_of_output = line_of_output + first_negative_coefficients[degree] * second_coefficients[two_degree] + "/x+";
                            }
                        }
                    }
                }
                if (line_of_output[line_of_output.Length - 1] == '+')
                {
                    line_of_output = line_of_output.Remove(line_of_output.Length - 1, 1);
                }
                line_of_output = line_of_output.Replace("+-", "-");
                line_of_output = line_of_output + "=";
                equation = line_of_output;
                solution_of_equation();
                line_of_output = "";
                for (degree = 9; degree > -1; degree--)
                {
                    if (coefficients[degree] > 0)
                    {
                        line_of_output = line_of_output + "+";
                    }
                    if (coefficients[degree] != 0)
                    {
                        if (degree > 1)
                        {
                            line_of_output = line_of_output + coefficients[degree] + "x^" + degree;
                        }
                        if (degree == 1)
                        {
                            line_of_output = line_of_output + coefficients[degree] + "x";
                        }
                        if (degree == 0)
                        {
                            line_of_output = line_of_output + coefficients[degree] + "";
                        }
                    }
                }
                for (degree = 9; degree > 0; degree--)
                {
                    if (negative_coefficients[degree] > 0)
                    {
                        line_of_output = line_of_output + "+";
                    }
                    if (negative_coefficients[degree] != 0)
                    {
                        if (degree > 1)
                        {
                            line_of_output = line_of_output + coefficients[degree] + "/x^" + degree;
                        }
                        if (degree == 1)
                        {
                            line_of_output = line_of_output + coefficients[degree] + "/x";
                        }
                    }
                }
                if (line_of_output[0] == '+')
                {
                    line_of_output = line_of_output.Remove(0, 1);
                }
                equation = equation_two.Remove(second_bracket_1 + 1, second_bracket_2 - second_bracket_1);
                equation = equation.Remove(first_bracket_1 + 1, second_bracket_1 - first_bracket_1 - 1);
                equation = equation.Insert(first_bracket_1 + 1, line_of_output);
                degree = first_bracket_1 + 1;
                while (equation[degree] != ')')
                {
                    degree++;
                }
                first_bracket = first_bracket_1;
                second_bracket = degree;
            }
            else
            {
                n = 0;
                for (degree = 9; degree > -1; degree--)
                {
                    if (second_coefficients[degree] != 0)
                    {
                        n = n + 1;
                    }
                    if (second_negative_coefficients[degree] != 0)
                    {
                        n = n + 1;
                    }
                }
                if (n == 1)
                {
                    line_of_output = "";
                    if (max_negative_degree_2 > 0)
                    {
                        for (degree = max_negative_degree_1; degree > 0; degree--)
                        {
                            if (first_negative_coefficients[degree] != 0)
                            {
                                if (first_negative_coefficients[degree] > 0)
                                {
                                    line_of_output = line_of_output + "+";
                                }
                                if (degree + max_negative_degree_2 > 1)
                                {
                                    line_of_output = line_of_output + first_negative_coefficients[degree] / second_negative_coefficients[max_negative_degree_2] + "/x^" + degree + max_negative_degree_2;
                                }
                                if (degree + max_negative_degree_2 == 1)
                                {
                                    line_of_output = line_of_output + first_negative_coefficients[degree] / second_negative_coefficients[max_negative_degree_2] + "/x";
                                }
                            }
                        }
                        for (degree = max_negative_degree_2; degree > -1; degree--)
                        {
                            if (first_coefficients[degree] != 0)
                            {
                                if (first_coefficients[degree] > 0)
                                {
                                    line_of_output = line_of_output + "+";
                                }
                                if (degree - max_negative_degree_2 > 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_negative_coefficients[max_degree_2] + "/x^" + (degree - max_negative_degree_2);
                                }
                                if (degree - max_negative_degree_2 == 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_negative_coefficients[max_degree_2] + "/x";
                                }
                                if (degree - max_negative_degree_2 == 0)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_negative_coefficients[max_degree_2];
                                }
                            }
                        }
                        for (degree = max_degree_1; degree > max_negative_degree_2; degree--)
                        {
                            if (first_coefficients[degree] != 0)
                            {
                                if (first_coefficients[degree] > 0)
                                {
                                    line_of_output = line_of_output + "+";
                                }
                                if (degree - max_negative_degree_2 > 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_negative_coefficients[max_negative_degree_2] + "x^" + (degree - max_negative_degree_2);
                                }
                                if (degree - max_negative_degree_2 == 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_negative_coefficients[max_negative_degree_2] + "x";
                                }
                            }
                        }
                        if (line_of_output[0] == '+')
                        {
                            line_of_output = line_of_output.Remove(0, 1);
                        }
                    }
                    if (max_degree_2 > 0)
                    {
                        for (degree = max_degree_1; degree > -1; degree--)
                        {
                            if (first_coefficients[degree] != 0)
                            {
                                if (first_coefficients[degree] > 0)
                                {
                                    line_of_output = line_of_output + "+";
                                }
                                if (degree + max_degree_2 > 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_coefficients[max_degree_2] + "x^" + degree + max_degree_2;
                                }
                                if (degree + max_degree_2 == 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_coefficients[max_degree_2] + "x";
                                }
                            }
                        }
                        for (degree = max_degree_2; degree > -1; degree--)
                        {
                            if (first_negative_coefficients[degree] != 0)
                            {
                                if (first_negative_coefficients[degree] > 0)
                                {
                                    line_of_output = line_of_output + "+";
                                }
                                if (max_degree_2 - degree > 1)
                                {
                                    line_of_output = line_of_output + first_negative_coefficients[degree] / second_coefficients[max_degree_2] + "x^" + (max_degree_2 - degree);
                                }
                                if (max_degree_2 - degree == 1)
                                {
                                    line_of_output = line_of_output + first_negative_coefficients[degree] / second_coefficients[max_degree_2] + "x";
                                }
                                if (max_degree_2 - degree == 0)
                                {
                                    line_of_output = line_of_output + first_negative_coefficients[degree] / second_coefficients[max_degree_2];
                                }
                            }
                        }
                        for (degree = max_negative_degree_1; degree > max_degree_2; degree--)
                        {
                            if (first_negative_coefficients[degree] != 0)
                            {
                                if (first_negative_coefficients[degree] > 0)
                                {
                                    line_of_output = line_of_output + "+";
                                }
                                if (degree - max_degree_2 > 1)
                                {
                                    line_of_output = line_of_output + first_negative_coefficients[degree] / second_coefficients[max_degree_2] + "/x^" + (degree - max_degree_2);
                                }
                                if (degree - max_degree_2 == 1)
                                {
                                    line_of_output = line_of_output + first_negative_coefficients[degree] / second_coefficients[max_degree_2] + "/x";
                                }
                            }
                        }
                        if (line_of_output[0] == '+')
                        {
                            line_of_output = line_of_output.Remove(0, 1);
                        }
                    }
                    if (max_degree_2 == 0)
                    {
                        for (degree = max_degree_1; degree > -1; degree--)
                        {
                            if (first_coefficients[degree] != 0)
                            {
                                if (first_coefficients[degree] > 0)
                                {
                                    line_of_output = line_of_output + "+";
                                }
                                if (degree > 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_coefficients[max_degree_2] + "x^" + degree;
                                }
                                if (degree == 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_coefficients[max_degree_2] + "x";
                                }
                                if (degree == 0)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_coefficients[max_degree_2];
                                }
                            }
                        }
                        for (degree = max_negative_degree_1; degree > 0; degree--)
                        {
                            if (first_coefficients[degree] != 0)
                            {
                                if (first_coefficients[degree] > 0)
                                {
                                    line_of_output = line_of_output + "+";
                                }
                                if (degree > 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_coefficients[max_degree_2] + "/x^" + degree;
                                }
                                if (degree == 1)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_coefficients[max_degree_2] + "/x";
                                }
                                if (degree == 0)
                                {
                                    line_of_output = line_of_output + first_coefficients[degree] / second_coefficients[max_degree_2];
                                }
                            }
                        }
                        if (line_of_output[0] == '+')
                        {
                            line_of_output = line_of_output.Remove(0, 1);
                        }
                    }
                }
                else
                {
                    division_of_brackets(first_coefficients, first_negative_coefficients, second_coefficients, second_negative_coefficients);
                    equation = equation_two.Remove(second_bracket_1 + 1, second_bracket_2 - second_bracket_1);
                    equation = equation.Remove(first_bracket_1 + 1, second_bracket_1 - first_bracket_1 - 1);
                    equation = equation.Insert(first_bracket_1 + 1, line_of_output);
                    degree = first_bracket_1 + 1;
                    while (equation[degree] != ')')
                    {
                        degree++;
                    }
                    first_bracket = first_bracket_1;
                    second_bracket = degree;
                }

            }
            line_of_output = "";
            equation_two = "";
            for (degree = 9; degree > -1; degree--)
            {
                coefficients[degree] = 0;
                negative_coefficients[degree] = 0;
            }
            degree = 0;
            max_degree = 0;
            max_negative_degree = 0;
        }

        public void division_of_brackets(double[] first_coefficients, double[] first_negative_coefficients, double[] second_coefficients, double[] second_negative_coefficients)
        {
            double[] common_first_coefficients = new double[20];
            double[] common_second_coefficients = new double[20];
            int first_max_degree = 0, second_max_degree = 0;
            int first_min_degree = 0, second_min_degree = 0;
            double multiplier;
            int multiplier_degree;
            line_of_output = "";
            for (degree = 9; degree > 0; degree--)
            {
                common_first_coefficients[10 - degree] = first_negative_coefficients[degree];
                common_second_coefficients[10 - degree] = second_negative_coefficients[degree];
            }
            for (degree = 0; degree < 10; degree++)
            {
                common_first_coefficients[10 + degree] = first_coefficients[degree];
                common_second_coefficients[10 + degree] = second_coefficients[degree];
            }
            for (degree = 19; degree > 0; degree--)
            {
                if (common_first_coefficients[degree] != 0 && first_max_degree == 0)
                {
                    first_max_degree = degree;
                }
                if (common_second_coefficients[degree] != 0 && second_max_degree == 0)
                {
                    second_max_degree = degree;
                }
                if (first_max_degree != 0 && second_max_degree != 0)
                {
                    break;
                }
            }
            for (degree = 0; degree < 20; degree++)
            {
                if (common_first_coefficients[degree] != 0 && first_min_degree == 0)
                {
                    first_min_degree = degree;
                }
                if (common_second_coefficients[degree] != 0 && second_min_degree == 0)
                {
                    second_min_degree = 0;
                }
                if (first_min_degree != 0 && second_min_degree != 0)
                {
                    break;
                }
            }
            for (degree = first_max_degree; degree >= second_max_degree; degree--)
            {
                multiplier = common_first_coefficients[degree] / common_second_coefficients[second_max_degree];
                multiplier_degree = degree - second_max_degree;
                if (multiplier > 0)
                {
                    for (index = degree; index >= first_min_degree; index--)
                    {
                        common_first_coefficients[index] = common_first_coefficients[index] - common_second_coefficients[index - multiplier_degree] * multiplier;
                    }
                    if (line_of_output.Length > 1 && multiplier > 0)
                    {
                        line_of_output = line_of_output + '+';
                    }
                    line_of_output = line_of_output + multiplier;
                    if (multiplier_degree > 1)
                    {
                        line_of_output = line_of_output + "x^" + multiplier_degree;
                    }
                    if (multiplier_degree == 1)
                    {
                        line_of_output = line_of_output + 'x';
                    }
                    if (multiplier_degree == -1)
                    {
                        line_of_output = line_of_output + "/x";
                    }
                    if (multiplier_degree < -1)
                    {
                        line_of_output = line_of_output + "/x^" + Math.Abs(multiplier_degree);
                    }
                }
            }
            for (degree = 1; degree < 20; degree--)
            {
                if (common_first_coefficients[degree] != 0)
                {
                    throw new Exception("division is impossible");
                }
            }
        }

        public void construction_of_an_expression_in_degree()
        {
            line_of_output = equation.Substring(first_bracket, second_bracket - first_bracket + 1);
            int n = second_bracket + 2;
            if (equation[second_bracket + 2] == '(')
            {
                int first_bracket_1 = first_bracket, second_bracket_1 = second_bracket;
                n = 1;
                for (degree = second_bracket + 3; degree < equation.Length; degree++)
                {
                    if (equation[degree] == '(')
                    {
                        n = n + 1;
                    }
                    if (equation[degree] == ')')
                    {
                        n = n - 1;
                    }
                    if (n == 0)
                    {
                        break;
                    }
                    if (equation[degree] == '=')
                    {
                        throw new Exception("unacceptable sign in brakcets");
                    }
                    if (degree == equation.Length - 1)
                    {
                        throw new Exception("unpared brakcets");
                    }
                }
                string equation_two = equation;
                equation_two = equation_two.Remove(second_bracket + 1, degree - second_bracket);
                first_bracket = second_bracket + 2;
                second_bracket = degree;
                degree = 0;
                equation = equation.Substring(first_bracket + 1, second_bracket - first_bracket - 1) + "=";
                executing_actions_with_brackets();
                solution_of_equation();
                for (n = 9; n > 0; n--)
                {
                    if (coefficients[n] != 0)
                    {
                        throw new Exception("wrong degree");
                    }
                }
                degree = (int)Math.Round(coefficients[0]);
                if (degree != coefficients[0])
                {
                    throw new Exception("wrong degree two");
                }
                coefficients[0] = 0;
                equation = equation_two;
                first_bracket = first_bracket_1;
                second_bracket = second_bracket_1;
                if (degree < 0)
                {
                    degree = Math.Abs(degree);
                    for (n = 1; n < degree; n++)
                    {
                        equation = equation.Insert(second_bracket + 1, "/" + line_of_output);
                    }
                }
                if (degree > 0)
                {
                    for (n = 1; n < degree; n++)
                    {
                        equation = equation.Insert(second_bracket + 1, "*" + line_of_output);
                    }
                }
                if (degree == 0)
                {
                    equation = equation.Remove(first_bracket + 1, second_bracket - first_bracket - 1).Insert(first_bracket + 1, "1");
                    second_bracket = first_bracket + 2;
                }
            }
            else
            {
                while (n < equation.Length - 1)
                {
                    if (equation[n] == 'x')
                    {
                        throw new Exception("wrong degree");
                    }
                    if (equation[n] == '=')
                    {
                        break;
                    }
                    if (equation[n] == '*')
                    {
                        break;
                    }
                    if (equation[n] == '/')
                    {
                        break;
                    }
                    if (equation[n] == '-' && n > second_bracket + 2)
                    {
                        break;
                    }
                    if (equation[n] == '+' && n > second_bracket + 2)
                    {
                        break;
                    }
                    n++;
                }
                degree = Int32.Parse(equation.Substring(second_bracket + 2, n - second_bracket - 2));
                equation = equation.Remove(second_bracket + 1, n - second_bracket - 1);
                if (degree < 0)
                {
                    degree = Math.Abs(degree);
                    for (n = 1; n < degree; n++)
                    {
                        equation = equation.Insert(second_bracket + 1, "/" + line_of_output);
                    }
                }
                if (degree > 0)
                {
                    for (n = 1; n < degree; n++)
                    {
                        equation = equation.Insert(second_bracket + 1, "*" + line_of_output);
                    }
                }
                if (degree == 0)
                {
                    equation = equation.Remove(first_bracket + 1, second_bracket - first_bracket - 1).Insert(first_bracket + 1, "1");
                    second_bracket = first_bracket + 2;
                }
            }
            line_of_output = "";
            degree = 0;
        }

        public void sign_change_in_brakets(int index_1, int index_2)
		{
			for (int n = index_1 + 1; n < index_2; n++)
			{
				if (equation[n] == '+')
				{
					equation = equation.Remove(n, 1).Insert(n, "-");
					continue;
				}
				if (equation[n] == '-')
				{
					equation = equation.Remove(n, 1).Insert(n, "+");
					continue;
				}
			}
			if (equation[index_1 + 1] != '-' && equation[index_1 + 1] != '+')
			{
				equation = equation.Insert(index_1 + 1, "-");
				second_bracket += 1;
			}
			equation = equation.Remove(index_1 - 1, 1);
			first_bracket -= 1;
			second_bracket -= 1;
		}
		
		public void performing_multiplication_before_brakets()
		{
			multiplier_degree = 0;
			multiplier = 1; 
			// нахождение позиции первого множителя
			for (character_number = first_bracket - 2; character_number > -1; character_number--)
			{
				if (character_number == 0) 
				{
					character_number = -1;
					break;
				}
				if (equation[character_number] == '=') break;
				if (equation[character_number] == '+' && equation[character_number - 1] != '/' && equation[character_number - 1] != '*') break;
				if (equation[character_number] == '-' && equation[character_number - 1] != '/' && equation[character_number - 1] != '*') break;
			}
			if (character_number > -1 && equation[character_number] == '-') 
			{
				equation = equation.Remove(character_number, 1).Insert(character_number, "+");
				multiplier = - multiplier;
			}
			// нахождение первого множителя
			for (character = character_number + 1; character_number <= first_bracket; character++)
			{
				// если первый множитель - переменная
				if (equation[character] == 'x')
				{
					// если коэффициент при первом множителем равен 1
					if (character_number + 1 == character)
					{
						multiplier = multiplier * 1;
					}
					else
					{
						multiplier = multiplier * Double.Parse(equation.Substring(character_number + 1, character - character_number - 1));
					}
					// если степень переменной > 1
					if (equation[character + 1] == '^')
					{
						multiplier_degree = multiplier_degree + Int32.Parse(equation.Substring(character + 2, 1));                                               
						character = character + 3;
					}
					else
					{
						multiplier_degree = multiplier_degree + 1;
						character = character + 1;
					}
				break;
				}
				// вычисление первого множителя если он не является переменной
				if (equation[character] == '*' || equation[character] == '/')
				{
					multiplier = multiplier * Double.Parse(equation.Substring(character_number + 1, character - character_number - 1));
					break;
				}
			}
			character_index = character;
			character += 1;
			while (character <= first_bracket)
            {
				// если множитель - переменная
				if (equation[character] == 'x')
				{
					// если на переменную умножают
					if (equation[character_index] == '*')
					{
						// если коэффициент при первом множителем равен 1
						if (equation[character_index + 1] == 'x')
						{
							multiplier = multiplier * 1;
						}
						else
						{
							multiplier = multiplier * Double.Parse(equation.Substring(character_index + 1, character - character_index - 1));
						}
						// если степень переменной > 1
						if (equation[character + 1] == '^')
						{
							multiplier_degree = multiplier_degree + Int32.Parse(equation.Substring(character + 2, 1));
							character_index = character + 3;
							character = character + 3;
						}
						else
						{
							multiplier_degree += 1;
							character_index = character + 1;
							character = character + 1;
						}			
					}
					// если на переменную делят
					else
					{
						// если коэффициент при первом множителем равен 1
						if (equation[character_index + 1] == 'x')
						{
							multiplier = multiplier / 1;
						}
						else
						{
							multiplier = multiplier * Double.Parse(equation.Substring(character_index + 1, character - character_index - 1));
						}
						// если степень переменной > 1
						if (equation[character + 1] == '^')
						{
							multiplier_degree = multiplier_degree - Int32.Parse(equation.Substring(character + 2, 1));
							character_index = character + 3;
							character = character + 3;
						}
						else
						{
							multiplier_degree = multiplier_degree - 1;
							character_index = character + 1;
							character = character + 1;
						}
					}
				}
				// если множитель - число
				else
				{
					// если множитель последний
					if (character == first_bracket)
					{
						break;
					}
					// если множитель не последний и перед ним знак *
					if ((equation[character] == '*' || equation[character] == '/') && equation[character_index] == '*')
					{
						multiplier = multiplier * Double.Parse(equation.Substring(character_index + 1, character - character_index - 1));
						character_index = character;
					}
					// если множитель не последний и перед ним знак /
					if ((equation[character] == '*' || equation[character] == '/') && equation[character_index] == '/')
					{
						multiplier = multiplier  / Double.Parse(equation.Substring(character_index + 1, character - character_index - 1));
						character_index = character;
					}
				}
				character = character + 1;
			}
			int first_index = character_number + 1;
			character = 0;
			character_index = 0;
			character_number = 0;
			string equation_two = equation;
			equation = equation.Substring(first_bracket + 1, second_bracket - first_bracket - 1) + "=";
			int second_index = equation.Length - 1;
			solution_of_equation();
			// определение максимальной степени в уравнении
 			for (degree = 9; degree > 0; degree--)
 			{
 				if (coefficients[degree] != 0)
 				{
 					max_degree = degree;
 					break;
 				}
 			}
			// определение максимальной отрицательной степени в уравнении
 			for (degree = 9; degree > 0; degree--)
 			{
 				if (negative_coefficients[degree] != 0)
 				{
 					max_negative_degree = degree;
 					break;
 				}
 			}
 			// если после умножения уравнения на переменную в максимальной отрицательной степени степень переменной > 9
 			if ((multiplier_degree > 0 && max_degree + multiplier_degree > 9) || (multiplier_degree < 0 && Math.Abs(multiplier_degree) + max_negative_degree > 9))
 			{
				throw new Exception("big degree");
 			}
 			if (equation_two[first_bracket - 1] == '*')
 			{
 				if (multiplier_degree == 0)
 				{
 					for (degree = max_degree; degree > 0; degree--)
 					{
 						if (coefficients[degree] != 0)
 						{
 							line_of_output += coefficients[degree] * multiplier + "x";
 							if (degree > 1) 
 							{
 								line_of_output = line_of_output + "^" + degree;
 							}
 							if (coefficients[degree - 1] >= 0)
 							{
 								line_of_output += "+";
 							}
 						}
 					}
 					for (degree = max_negative_degree; degree > 0; degree--)
 					{
 						if (negative_coefficients[degree] != 0)
 						{
 							line_of_output += negative_coefficients[degree] * multiplier + "/" + "x";
 							if (degree > 1) 
 							{
 								line_of_output = line_of_output + "^" + degree;
 							}	
 							if (negative_coefficients[degree - 1] >= 0)
 							{
 								line_of_output += "+";
 							}
 						}
 					}
 					line_of_output += coefficients[0] * multiplier;
 				}
 				if (multiplier_degree > 0)
 				{
 					for (degree = max_degree; degree >= 0; degree--)
 					{
 						coefficients[degree + multiplier_degree] = coefficients[degree];
 					}
 					for (degree = 0; degree < multiplier_degree; degree++)
 					{
 						coefficients[degree] = negative_coefficients[multiplier_degree - degree];
 					}
 					for (degree = max_negative_degree; degree > max_negative_degree; degree--)
 					{
 						negative_coefficients[degree] = negative_coefficients[degree - multiplier_degree];
 						negative_coefficients[degree] = 0;
 					}
 					max_degree += multiplier_degree;
 					max_negative_degree -= multiplier_degree;
 					for (degree = max_degree; degree > 0; degree--)
 					{
 						if (coefficients[degree] != 0)
 						{
 							line_of_output += coefficients[degree] * multiplier + "x";
 							if (degree > 1) 
 							{
 								line_of_output = line_of_output + "^" + degree;
 							}
 							if (coefficients[degree - 1] >= 0)
 							{
 								line_of_output += "+";
 							}
 						}
 					}
 					for (degree = max_negative_degree; degree > 0; degree--)
 					{
 						if (negative_coefficients[degree] != 0)
 						{
 							line_of_output += negative_coefficients[degree] * multiplier + "/" + "x";
 							if (degree > 1) 
 							{
 								line_of_output = line_of_output + "^" + degree;
 							}	
 							if (negative_coefficients[degree - 1] >= 0)
 							{
 								line_of_output += "+";
 							}
 						}
 					}
 					line_of_output += coefficients[0] * multiplier;
 				}
 				else
 				{
 					multiplier_degree = - multiplier_degree;
 					for (degree = max_negative_degree; degree > 0; degree--)
 					{
 						negative_coefficients[degree + multiplier_degree] = negative_coefficients[degree];
 					}
 					for (degree = 0; degree <= multiplier_degree; degree++)
 					{
 						negative_coefficients[degree] = coefficients[multiplier_degree - degree];
 					}
 					for (degree = max_degree; degree >= multiplier_degree; degree--)
 					{
 						coefficients[degree] = coefficients[degree - multiplier_degree];
 						coefficients[degree] = 0;
 					}
 					coefficients[0] = negative_coefficients[0];
 					negative_coefficients[0] = 0;
 					max_degree -= multiplier_degree;
 					max_negative_degree += multiplier_degree;
 					for (degree = max_degree; degree > 0; degree--)
 					{
 						if (coefficients[degree] != 0)
 						{
 							line_of_output += coefficients[degree] * multiplier + "x";
 							if (degree > 1) 
 							{
 								line_of_output = line_of_output + "^" + degree;
 							}
 							if (coefficients[degree - 1] >= 0)
 							{
 								line_of_output += "+";
 							}
 						}
 					}
 					for (degree = max_negative_degree; degree > 0; degree--)
 					{
 						if (negative_coefficients[degree] != 0)
 						{
 							line_of_output += negative_coefficients[degree] * multiplier + "/" + "x";
 							if (degree > 1) 
 							{
 								line_of_output = line_of_output + "^" + degree;
 							}	
 							if (negative_coefficients[degree - 1] >= 0)
 							{
 								line_of_output += "+";
 							}
 						}
 					}
 				line_of_output += coefficients[0] * multiplier;
 				}
 			}
 			else
 			{
 				int k = 0;
 				for (degree = 9; degree >= 0; degree--)
 				{
 					if (coefficients[degree] != 0)
 					{
 						k = k + 1;
 						max_degree = degree;
 					}
 					if (negative_coefficients[degree] != 0)
 					{
 						k = k + 1;
 						max_negative_degree = degree;
 					}
 				}
 				if (k == 1)
 				{
 					if (max_degree > 0)
 					{
 						if (multiplier_degree > 0)
 						{
 							if (max_degree - multiplier_degree == 0)
 							{
 								line_of_output = multiplier / coefficients[max_degree] + "";
 							}
 							if (max_degree - multiplier_degree > 0)
 							{
 								line_of_output = multiplier / coefficients[max_degree] + "/x^" + (max_degree - multiplier_degree);
 							}
 							else
 							{
 								line_of_output = multiplier / coefficients[max_degree] + "x^" + (multiplier_degree - max_degree);
 							}
 						}
 						if (multiplier_degree < 0)
 						{
 							line_of_output = multiplier / coefficients[max_degree] + "/x^" + (max_degree + multiplier_degree);
 						}
 						if (multiplier_degree == 0)
 						{
 							line_of_output = multiplier / coefficients[max_degree] + "/x^" + max_degree;
 						}
 					}
 					if (max_negative_degree > 0)
 					{
 						if (multiplier_degree > 0)
 						{
 							line_of_output = multiplier / coefficients[max_negative_degree] + "x^" + (max_negative_degree + multiplier_degree);
 						}
 						if (multiplier_degree < 0)
 						{
 							if (max_negative_degree + multiplier_degree == 0)
 							{
 								line_of_output = multiplier / coefficients[max_degree] + "";
 							}
 							if (max_negative_degree + multiplier_degree > 0)
 							{
 								line_of_output = multiplier / coefficients[max_degree] + "x^" + (max_negative_degree + multiplier_degree);
 							}
 							else
 							{
 								line_of_output = multiplier / coefficients[max_degree] + "/x^" + (Math.Abs(multiplier_degree) - max_negative_degree);
 							}
 						}
 						if (multiplier_degree == 0)
 						{
 							line_of_output = multiplier / coefficients[max_negative_degree] + "x^" + max_negative_degree;
 						}
 					}
 					if (max_degree == 0 && max_negative_degree == 0)
 					{
 						if (multiplier_degree > 0)
 						{
 							line_of_output = multiplier / coefficients[max_degree] + "x^" + multiplier_degree;
 						}
 						if (multiplier_degree < 0)
 						{
 							line_of_output = multiplier / coefficients[max_degree] + "/x^" + (- multiplier_degree);
 						}
 						if (multiplier_degree == 0)
 						{
 							line_of_output = multiplier / coefficients[max_degree] + "";
 						}
 					}
 				}
 				else
 				{
                    double[] first_coefficients = new double[10];
                    double[] first_negative_coefficients = new double[10];
                    double[] second_coefficients = coefficients;
                    double[] second_negative_coefficients = negative_coefficients;
                    if (multiplier_degree == 0)
                    {
                        first_coefficients[0] = multiplier;
                    }
                    if (multiplier_degree > 0)
                    {
                        first_coefficients[multiplier_degree] = multiplier;
                    }
                    if (multiplier_degree < 0)
                    {
                        first_negative_coefficients[Math.Abs(multiplier_degree)] = multiplier; 
                    }
                    division_of_brackets(first_coefficients, first_negative_coefficients, second_coefficients, second_negative_coefficients);
 				}
 			}
 			for (degree = 9; degree >= 0; degree--)
 			{
 				coefficients[degree] = 0;
 				negative_coefficients[degree] = 0;
 			}
 			equation = equation_two;
 			equation = equation.Remove(first_bracket + 1, second_bracket - first_bracket - 1).Insert(first_bracket + 1, line_of_output);
 			second_bracket = second_bracket - (second_index - line_of_output.Length);
 			equation = equation.Remove(first_index, first_bracket - first_index);
 			second_bracket = second_bracket - (first_bracket - first_index);
 			first_bracket = first_bracket - (first_bracket - first_index);
 			max_negative_degree = 0;
 			max_degree = 0;
 			multiplier_degree = 0;
 			multiplier = 1;
 			character = 0;
			character_index = 0;
			character_number = 0;
		}
		
		public void performing_multiplication_after_brakets()
		{
			multiplier_degree = 0;
			multiplier = 1;
			while (equation[second_bracket + 1] == '*' || equation[second_bracket + 1] == '/')
			{
				for (index = second_bracket + 2; index < equation.Length; index++)
				{
					if (equation[index] == 'x')
					{
						if (equation[second_bracket + 1] == '*')
						{
							if (second_bracket + 2 != index)
							{
								multiplier = multiplier * Double.Parse(equation.Substring(second_bracket + 2, index - second_bracket - 2));
							}
							if (index + 1 < equation.Length && equation[index + 1] == '^')
							{
								multiplier_degree = multiplier_degree + Int32.Parse(equation[index + 2].ToString());
								equation = equation.Remove(index, 3);
							}
							else
							{
								multiplier_degree = multiplier_degree + 1;
								equation = equation.Remove(index, 1);
							}
						}
						else
						{
							if (second_bracket + 2 != index)
							{
								multiplier = multiplier / Double.Parse(equation.Substring(second_bracket + 2, index - second_bracket - 2));
							}
							if (index + 1 < equation.Length && equation[index + 1] == '^')
							{
								multiplier_degree = multiplier_degree - Int32.Parse(equation[index + 2].ToString());
								equation = equation.Remove(index, 3);
							}
							else
							{
								multiplier_degree = multiplier_degree - 1;
								equation = equation.Remove(index, 1);
							}
						}
						break;
					}
					if (equation[index] == '*' || equation[index] == '/' || (equation[index] == '+' && (equation[index] != '*' || equation[index] != '/')) || (equation[index] == '-' && (equation[index] != '*' || equation[index] != '/')) || equation[index] == '=' || index == equation.Length - 1)
					{
						if (equation[second_bracket + 1] == '*')
						{
							multiplier = multiplier * Double.Parse(equation.Substring(second_bracket + 2, index - second_bracket - 2));
						}
						else
						{
							multiplier = multiplier / Double.Parse(equation.Substring(second_bracket + 2, index - second_bracket - 2));
						}
						break;
					}
				}
				equation = equation.Remove(second_bracket + 1, index - second_bracket - 1);
			}
			index = 0;
			string equation_two = equation;
			equation = equation.Substring(first_bracket + 1, second_bracket - first_bracket - 1) + "=";
			int length = equation.Length - 1;
			solution_of_equation();
			equation = equation_two;
			for (degree = 9; degree >= 0; degree--)
			{
				if (coefficients[degree] != 0)
				{
					max_degree = degree;
					break;
				}
			}
			for (degree = 9; degree >= 0; degree--)
			{
				if (negative_coefficients[degree] != 0)
				{
					max_negative_degree = degree;
					break;
				}
			}
			line_of_output = "";
			if ((multiplier_degree > 0 && max_degree + multiplier_degree > 9) || (multiplier_degree < 0 && Math.Abs(multiplier_degree) + max_negative_degree > 9))
 			{
 				throw new Exception("big degree");
 			}
			if (multiplier_degree == 0)
			{
				for (degree = max_degree; degree > 0; degree--)
				{
					if (coefficients[degree] != 0)
					{
						line_of_output = line_of_output + coefficients[degree] * multiplier + "x";
						if (degree > 1)
						{
							line_of_output = line_of_output + "^" + degree;
						}
						}
					if (coefficients[degree - 1] > 0)
					{
						line_of_output = line_of_output + "+";
					}
				}
				line_of_output = line_of_output + coefficients[0] * multiplier;
				for (degree = max_negative_degree; degree > 0; degree--)
				{
					if (negative_coefficients[degree] > 0)
					{
						line_of_output = line_of_output + "+";
					}
					if (negative_coefficients[degree] != 0)
					{
						line_of_output = line_of_output + negative_coefficients[degree] * multiplier + "/x";
						if (degree > 1)
						{
							line_of_output = line_of_output + "^" + degree;
						}
					}
				}
			}
			if (multiplier_degree > 0)
			{
				for (degree = max_degree; degree >= 0; degree--)
				{
					coefficients[degree + multiplier_degree] = coefficients[degree];
				}
				for (degree = multiplier_degree; degree > 0; degree--)
				{
					coefficients[multiplier_degree - degree] = negative_coefficients[degree];
				}
				for (degree = max_negative_degree; degree > multiplier_degree; degree--)
				{
					negative_coefficients[degree - multiplier_degree] = negative_coefficients[degree];
					negative_coefficients[degree] = 0;
				}
				max_degree = max_degree + multiplier_degree;
				max_negative_degree = max_negative_degree - multiplier_degree;
				for (degree = max_degree; degree > 0; degree--)
				{
					if (coefficients[degree] != 0)
					{
						line_of_output = line_of_output + coefficients[degree] * multiplier + "x";
						if (degree > 1)
						{
							line_of_output = line_of_output + "^" + degree;
						}
					}
					if (coefficients[degree - 1] > 0)
					{
						line_of_output = line_of_output + "+";
					}
				}
				if (coefficients[0] != 0)
				{
					line_of_output = line_of_output + coefficients[0] * multiplier;
				}
				for (degree = max_negative_degree; degree > 0; degree--)
				{
					if (negative_coefficients[degree] > 0)
					{
						line_of_output = line_of_output + "+";
					}
					if (negative_coefficients[degree] != 0)
					{
						line_of_output = line_of_output + negative_coefficients[degree] * multiplier + "/x";
						if (degree > 1)
						{
							line_of_output = line_of_output + "^" + degree;
						}
					}
				}
			}
			if (multiplier_degree < 0)
			{
				multiplier_degree = Math.Abs(multiplier_degree);
				for (degree = max_negative_degree; degree > 0; degree--)
				{
					negative_coefficients[degree + multiplier_degree] = negative_coefficients[degree];
				}
				for (degree = multiplier_degree; degree > 0; degree--)
				{
					negative_coefficients[degree] = coefficients[multiplier_degree - degree];
				}
				coefficients[0] = negative_coefficients[0];
				for (degree = max_degree; degree >= multiplier_degree; degree--)
				{
					coefficients[degree - multiplier_degree] = coefficients[degree];
					coefficients[degree] = 0;
				}
				max_degree = max_degree - multiplier_degree;
				max_negative_degree = max_negative_degree + multiplier_degree;
				for (degree = max_degree; degree > 0; degree--)
				{
					if (coefficients[degree] != 0)
					{
						line_of_output = line_of_output + coefficients[degree] * multiplier + "x";
						if (degree > 1)
						{
							line_of_output = line_of_output + "^" + degree;
						}
					}
					if (coefficients[degree - 1] > 0)
					{
						line_of_output = line_of_output + "+";
					}
				}
				line_of_output = line_of_output + coefficients[0] * multiplier;
				for (degree = max_negative_degree; degree > 0; degree--)
				{
					if (negative_coefficients[degree] > 0)
					{
						line_of_output = line_of_output + "+";
					}
					if (negative_coefficients[degree] != 0)
					{
						line_of_output = line_of_output + negative_coefficients[degree] * multiplier + "/x";
						if (degree > 1)
						{
							line_of_output = line_of_output + "^" + degree;
						}
					}
				}
			}
			equation = equation.Remove(first_bracket + 1, second_bracket - first_bracket - 1).Insert(first_bracket + 1, line_of_output);
			second_bracket = second_bracket - (length - line_of_output.Length);
			multiplier_degree = 0;
			multiplier = 1;
			line_of_output = "";
			max_degree = 0;
			max_negative_degree = 0;
			for (degree = 9; degree > -1; degree--)
			{
				coefficients[degree] = 0;
				negative_coefficients[degree] = 0;
			}
			degree = 0;
		}
		// нахождение коэффициента при х^n
		public void analysis_of_the_row(string variable)
		{
			double coefficient = 0;
			// пока х^n есть в строке
			while (equation.IndexOf(variable) != -1)
			{
				index = equation.IndexOf(variable);
				// если х^n стоит на первом месте в строке
				if (index == 0)
				{
					x_n = 1;
				}
				else
				{
					// если перед х^n стоит знак +
					if (equation[index - 1] == '+') 
					{
						x_n = 1;
						equation = equation.Remove(index - 1, 1);
					}
					// если перед х^n стоит знак -
					if (equation[index - 1] == '-') 
					{
						x_n = - 1;
						equation = equation.Remove(index - 1, 1);
					}
					//если перед х^n стоит знак =
					if (equation[index - 1] == '=') 
					{
						x_n = 1;
					}
                    index = equation.IndexOf(variable);
                    if (index > 0)
                    {
                    	// если перед x^n стоит еще один знак +
                    	if (equation[index - 1] == '+') 
                    	{
                    		equation = equation.Remove(index - 1, 1);
                    	}
                    	// если перед x^n стоит еще один знак -
                    	if (equation[index - 1] == '-') 
                   		{
                    		x_n = - 1; 
                    		equation = equation.Remove(index - 1, 1);
                    	}	
                    }
				}
				// нахождение коэффициента при текущей переменной
				if (x_n == 0)
				{
               		// определение количества цифр в текущем коэффициенте при x^n и его вычисление
               		for (character_number = index; character_number > -1; character_number--)
               		{
               			// если перед коэффициентом стоит знак =
               			if (equation[character_number] == '=')
               			{
               				x_n = Double.Parse(equation.Substring(character_number + 1, index - character_number - 1));
               				equation = equation.Remove(character_number + 1, index - character_number - 1);
               				index = equation.IndexOf(variable);
               				break;
               			}
               			// если первая цифра коэффициента стоит на 1 месте в строке 
               			if (character_number == 0)
               			{
               				x_n = double.Parse(equation.Substring(character_number, index - character_number));
               				equation = equation.Remove(character_number, index - character_number);
               				index = equation.IndexOf(variable);
               				break;
               			}
               			// если перед коэффициентом стоят знаки +-
               			if (character_number > 1 && equation[character_number - 1] == '-' && equation[character_number - 2] == '+')
               			{
               				x_n = Double.Parse(equation.Substring(character_number - 1, index - character_number + 1));
               				equation = equation.Remove(character_number - 2, index - character_number + 2);
               				index = equation.IndexOf(variable);
               				break;
               			}
               			// если перед коэффициентом стоят знаки -+
               			if (character_number > 1 && equation[character_number - 1] == '+' && equation[character_number - 2] == '-')
               			{
               				x_n = - Double.Parse(equation.Substring(character_number - 1, index - character_number + 1));
               				equation = equation.Remove(character_number - 2, index - character_number + 2);
               				index = equation.IndexOf(variable);
               				break;
               			}
               			// если перед коэффициентом стоят знаки ++
               			if (character_number > 1 && equation[character_number - 1] == '+' && equation[character_number - 2] == '+')
               			{
               				x_n = Double.Parse(equation.Substring(character_number - 1, index - character_number + 1));
               				equation = equation.Remove(character_number - 2, index - character_number + 2);
               				index = equation.IndexOf(variable);
               				break;
               			}
               			// если перед коэффициентом стоят знаки --
               			if (character_number > 1 && equation[character_number - 1] == '-' && equation[character_number - 2] == '-')
               			{
               				x_n = - Double.Parse(equation.Substring(character_number - 1, index - character_number + 1));
               				equation = equation.Remove(character_number - 2, index - character_number + 2);
               				index = equation.IndexOf(variable);
               				break;
               			}
               			// если перед коэффициентом стоит знак /
               			if (equation[character_number - 1] == '/') 
               			{
               				character_index = character_number - 1;
               				break;
               			}
               			// если перед коэффициентом стоит знак *
               			if (equation[character_number - 1] == '*') 
               			{
               				character_index = character_number - 1;
               				break;
               			}
               			// если перед коэффициентом стоит знак +
               			if (equation[character_number] == '+')
               			{
               				x_n = Double.Parse(equation.Substring(character_number + 1, index - character_number - 1));
               				equation = equation.Remove(character_number, index - character_number);
               				index = equation.IndexOf(variable);
               				break;
               			}
               			// если перед коэффициентом стоит знак -
               			if (equation[character_number] == '-')
               			{
               				x_n = - Double.Parse(equation.Substring(character_number + 1, index - character_number - 1));
               				equation = equation.Remove(character_number, index - character_number);
               				index = equation.IndexOf(variable);
               				break;
               			}
               			// если перед коэффициентом стоит знак /
               			if (equation[character_number] == '/')
               			{
               				character_index = character_number;
               				break;
               			}
               			// если перед коэффициентом стоит знак *
               			if (equation[character_number] == '*')
               			{
               				character_index = character_number;
               				break;
               			}
               		}
				}
				// если перед коэффициентом стоит знак / или *
				if (character_index > 0)
				{
					// нахождение позиции первого множителя
					for (character_number = character_index; character_number >= 0; character_number--)
					{
						if (character_number == 0) break;
						if (equation[character_number - 1] == '=') break;
						if (equation[character_number] == '+' && equation[character_number - 1] != '/' && equation[character_number - 1] != '*') break;
						if (equation[character_number] == '-' && equation[character_number - 1] != '/' && equation[character_number - 1] != '*') break;
					}
					x_n = 1;
					// удаление знака + перед коэффициентом
					if (equation[character_number] == '+') equation = equation.Remove(character_number, 1);
					// удаление знака - перед коэффициентом с умножением текущего коэффициента на - 1
					if (equation[character_number] == '-') 
					{
						equation = equation.Remove(character_number, 1);
						x_n = - x_n;
					}
					// определяем степень переменной после выподнения умножения
					current_degree = 0;
					// нахождение первого множителя
					for (character = character_number; character_number <= index; character++)
					{
						// если первый множитель - переменная
						if (equation[character] == 'x')
						{
							// если коэффициент при первом множителем равен 1
							if (equation[character_number] == 'x')
							{
								x_n = x_n * 1;
							}
							else
							{
								x_n = x_n * Double.Parse(equation.Substring(character_number, character - character_number));
							}
							// если степень переменной > 1
							if (equation[character_number + 1] == '^')
							{
								current_degree = current_degree + Int32.Parse(equation.Substring(character + 2, 1));
								equation = equation.Remove(character, 3);                                               
							}
							else
							{
								current_degree = current_degree + 1;
								equation = equation.Remove(character, 1);
							}
						// удаление первого множителя
						equation = equation.Remove(character_number, character - character_number);
						break;
						}
						// вычисление первого множителя если он не является переменной
						if (equation[character] == '*' || equation[character] == '/')
						{
							x_n = x_n * Double.Parse(equation.Substring(character_number, character - character_number));
							equation = equation.Remove(character_number, character - character_number);
							index = equation.IndexOf(variable);
							break;
						}
					}
					character = character - (character - character_number);
					character_index = character;
                    index = equation.IndexOf(variable);
                    character_number = character + 1;
                    // выполнение умножения / деления
                    while (character_number <= index)
                    {
                    	//if ((equation[index - 1] == '*' || equation[index - 1] == '/') && character_number > index - 1) break;
                    	// если множитель - переменная
                    	if (equation[character_number] == 'x' && character_number < equation.IndexOf(variable))
                    	{
                    		// если на переменную умножают
                    		if (equation[character_index] == '*')
                    		{
                    			// если коэффициент при первом множителем равен 1
                    			if (equation[character_index + 1] == 'x')
                    			{
                    				x_n = x_n * 1;
                    			}
                    			else
                    			{
                    				x_n = x_n * Double.Parse(equation.Substring(character_index + 1, character_number - character_index - 1));
                    			}
                    			// если степень переменной > 1
                    			if (equation[character_number + 1] == '^')
                    			{
                    				current_degree = current_degree + Int32.Parse(equation.Substring(character_number + 2, 1));
                    				character_index = character_number + 3;
                    				character_number = character_number + 3;
                    			}
                    			else 
                    			{
                    				current_degree = current_degree + 1; 
                    				character_index = character_number + 1;  
                    				character_number = character_number + 1; 
                    			}
                    				
                    		}
                    		// если на переменную делят
                    		else
                    		{
                    			// если коэффициент при первом множителем равен 1
                    			if (equation[character_index + 1] == 'x')
                    			{
                    				x_n = x_n / 1;
                    			}	
                    			else
                    			{
                    				x_n = x_n * Double.Parse(equation.Substring(character_index + 1, character_number - character_index - 1));
                    			}
                    			// если степень переменной > 1
                    			if (equation[character_number + 1] == '^')
                    			{
                    				current_degree = current_degree - Int32.Parse(equation.Substring(character_number + 2, 1));
                    				character_index = character_number + 3; 
                    				character_number = character_number + 3;	
                    			}
                    			else 
                    			{
                    				current_degree = current_degree - 1;
                    				character_index = character_number + 1;  
                    				character_number = character_number + 1;
                    			}
                    		}
                    	}
                    	// если множитель - число
                    	else
                    	{
                    		// если множитель не последний и перед ним знак *
                    		if ((equation[character_number] == '*' || equation[character_number] == '/') && equation[character_index] == '*')
                    		{
                    			x_n = x_n * Double.Parse(equation.Substring(character_index + 1, character_number - character_index - 1));
                    			character_index = character_number;
                    		}
                    		// если множитель не последний и перед ним знак /
                    		if ((equation[character_number] == '*' || equation[character_number] == '/') && equation[character_index] == '/')
                    		{
                    			x_n = x_n / Double.Parse(equation.Substring(character_index + 1, character_number - character_index - 1));
                    			character_index = character_number;
                    		}
                    		// если множитель последний и перед ним знак *
                    		if (character_number == index && equation[character_index] == '*')
                    		{
                    			// если степень переменной = 1
                    			if (variable.Length == 1)
                    			{
                    				current_degree = current_degree + 1;
                    			}
                    			else 
                    			{
                    				current_degree = current_degree + Int32.Parse(variable.Substring(2, 1));
                    			}
                    			// если коэффициент при переменной <> 1
                    			if (character_number - character_index > 1)
                    			{
                    				x_n = x_n * Double.Parse(equation.Substring(character_index + 1, character_number - character_index - 1));
                    			}
                    			break;
                    		}
                    		// если множитель последний и перед ним знак /
                    		if (character_number == index && equation[character_index] == '/')
                    		{
                    			// если коэффициент при переменной <> 1
                    			if (character_number - character_index > 1)
                    			{
                    				x_n = x_n / Double.Parse(equation.Substring(character_index + 1, character_number - character_index - 1));
                    			}
                    			// если степень переменной = 1
                    			if (variable.Length == 1)
                    			{
                    				current_degree = current_degree - 1;
                    			}
                    			else 
                    			{
                    				current_degree = current_degree - Int32.Parse(variable.Substring(2, 1));
                    			}
                    			break;
                    		}
                    	}
                    	character_number = character_number + 1;
                    }
                    equation = equation.Remove(character, equation.IndexOf(variable) - character);
                    index = equation.IndexOf(variable);
				}
				// если переменная стоит не на последнем месте в строке
				if (index + variable.Length < equation.Length)
				{
					character = index + variable.Length;
					character_number = index + variable.Length;
					// пока после переменной стоит знак / или * 
					while (equation[index + variable.Length] == '/' || equation[index + variable.Length] == '*')
					{
						character_number = character_number + 1;
						// если после знака стоит х
						if (character_number < equation.Length && equation[character_number] == 'x')
						{
							// если на х умножают
							if (equation[character] == '*')
							{
								// если коэффициент множителя = 1
								if (equation[character + 1] == 'x')
								{
									x_n = x_n * 1;
								}
								else
								{
									x_n = x_n * Double.Parse(equation.Substring(character + 1, character_number - character - 1));
								}
								// если степень переменной - множителя > 1
								if (character_number < equation.Length - 1 && equation[character_number + 1] == '^')
								{
									current_degree = current_degree + Int32.Parse(equation.Substring(character_number + 2, 1));
									equation = equation.Remove(character, character_number - character + 3);
									character_number = character;
								}
								else
								{
									current_degree = current_degree + 1;
									equation = equation.Remove(character, character_number - character + 1);
									character_number = character;
								}
							}
							// если на х делят
							else
							{
								// если коэффициент множителя = 1
								if (equation[character + 1] == 'x')
								{
									x_n = x_n / 1;
								}
								else
								{
									x_n = x_n / Double.Parse(equation.Substring(character + 1, character_number - character - 1));
								}
								// если степень переменной - множителя > 1
								if (character_number < equation.Length - 1 && equation[character_number + 1] == '^')
								{
									current_degree = current_degree - Int32.Parse(equation.Substring(character_number + 2, 1));
									equation = equation.Remove(character, character_number - character + 3);
									character_number = character;
								}
								else
								{
									current_degree = current_degree - 1;
									equation = equation.Remove(character, character_number - character + 1);
									character_number = character;
								}
							}
						}
						else
						{
							// если множитель - последнее число в строке
							if (character_number == equation.Length)
							{
								// если на множитель делят
								if (equation[character] == '/')
								{
									x_n = x_n / Double.Parse(equation.Substring(character + 1, character_number - character - 1));
									break;
								}
								else
								{
									x_n = x_n * Double.Parse(equation.Substring(character + 1, character_number - character - 1));
									break;
								}
							}
							// если перед множителем стоит знак +
							if (equation[character_number] == '+' && (equation[character_number - 1] == '/' || equation[character_number - 1] == '*'))
							{
								equation = equation.Remove(character_number, 1);
							}
							// если перед множителем стоит знак -
							if (equation[character_number] == '-' && (equation[character_number - 1] == '/' || equation[character_number - 1] == '*'))
							{
								x_n = - x_n;
								equation = equation.Remove(character_number, 1);
							}
							// если множитель стоит перед знаком =
							if (equation[character_number] == '=')
							{
								// если на множитель делят
								if (equation[character] == '/')
								{
									x_n = x_n / Double.Parse(equation.Substring(character + 1, character_number - character - 1));
									break;
								}
								else
								{
									x_n = x_n * Double.Parse(equation.Substring(character + 1, character_number - character - 1));
									break;
								}
							}
							// если множитель не последний и на него делят
							if (equation[character] == '/' && (equation[character_number] == '/' || equation[character_number] == '*'))
							{
								x_n = x_n / Double.Parse(equation.Substring(character + 1, character_number - character - 1));
								character = character_number;
							}
							// если множитель не последний и на него умножают
							if (equation[character] == '*' && (equation[character_number] == '/' || equation[character_number] == '*'))
							{
								x_n = x_n * Double.Parse(equation.Substring(character + 1, character_number - character - 1));
								character = character_number;
							}
							// если после множителя стоит знак + или -
							if ((equation[character_number] == '+' || equation[character_number] == '-') && (equation[character_number - 1] != '/' || equation[character_number - 1] != '*'))
							{
								// если на множитель делят
								if (equation[character] == '/')
								{
									x_n = x_n / Double.Parse(equation.Substring(character + 1, character_number - character - 1));
									break;
								}
								else
								{
									x_n = x_n * Double.Parse(equation.Substring(character + 1, character_number - character - 1));
									break;
								}
							}
						}
						// если множитель стоит на последнем месте в строке
						if (index + variable.Length > equation.Length - 1) break;
					}
					equation = equation.Remove(index + variable.Length, character_number - index - variable.Length);
				}
				// если переменная стоит после знака =
				if (index > equation.IndexOf('='))
				{
					x_n = - x_n;
				}
				equation = equation.Remove(equation.IndexOf(variable), variable.Length);
				coefficient = coefficient + x_n;
				if (current_degree < 0)
				{
					negative_coefficients[- current_degree] += coefficient;
				}
				else
				{
					coefficients[current_degree] += coefficient;
				}
				if (variable.Length > 1)
				{
					current_degree = Int32.Parse(variable.Substring(2, 1));
				}
				else
				{
					current_degree = 1;
				}
				index = 0;
				character_index = 0;
				character_number = 0;
				character = 0;
				x_n = 0;
				// если степень переменной < 0
				coefficient = 0;
			}
		}
		// нахождение свободного члена
		public double analysis_of_the_row_without_coefficients_x(string line)
		{
			int equally = -1;
			double resultoffunction = 0;
			// пока в строке есть свободные члены
			while (line.Length > 1)
			{
				// если на первом месте в строке стоит =
				if (line[0] == '=')
				{
					equally = 0;
				}
				// если перед числом стоит знак +
				if (line[equally + 1] == '+')
				{
					x_n = 1;
					line = line.Remove(equally + 1,1);
				}
				// если перед числом стоит знак -
				if (line[equally + 1] == '-')
				{
					x_n = -1;
					line = line.Remove(equally + 1,1); //delete(line,1,1);??????
				}
				// если перед числом знак не стоит
				if (x_n == 0)
				{
					x_n = 1;
				}
				// определение индекса последнего знака текущего числа в строке
				for (character_number = equally + 1; character_number < line.Length; character_number++)
				{
					if (line[character_number] == '+') break;
					if (line[character_number] == '=') break;
					if (line[character_number] == '-') break;
					if (line[character_number] == '*') break;
					if (line[character_number] == '/') break;
					if (character_number == line.Length - 1) break;
				}
				// если число стоит на последнем месте в строке перед знаком =
				if (line[character_number] == '=' && character_number == line.Length - 1)
				{
					// переменная равна текущему числу
					x_n = x_n * Double.Parse(line.Substring(equally + 1, character_number - equally - 1));
				}
				else
				{
					// если число стоит на последнем месте в строке
					if (character_number == line.Length - 1)
					{
						// переменная равна текущему числу
						x_n = x_n * Double.Parse(line.Substring(equally + 1, character_number - equally));
					}
					else
					{
						// переменная равна текущему числу
						x_n = x_n * Double.Parse(line.Substring(equally + 1, character_number - equally - 1));
					}
				}
				// пока после числа стоит знак * или /
				while (line.IndexOf('*') == character_number || line.IndexOf('/') == character_number)
				{
					// если после знака деления / умножения стоит знак +
					if (line[character_number + 1] == '+') line = line.Remove(character_number + 1, 1);
					// если после знака деления / умножения стоит знак -
					if (line[character_number + 1] == '-') 
					{
						// умножение переменной на -1 и удаление знака - перед вторым множителем
						x_n = - x_n;
						line = line.Remove(character_number + 1, 1);
					}
					// если после числа стоит знак *
					if (line[character_number] == '*')
					{
						// определение второго множителя
						for (character_index = character_number + 1; character_index < line.Length; character_index++)
						{
							if (line[character_index] == '+') break;
							if (line[character_index] == '-') break;
							if (line[character_index] == '=') break;
							if (line[character_index] == '*') break;
							if (line[character_index] == '/') break;
						}
						// если множитель - последнее число в строке
						if (character_index == line.Length)
						{
							// если после множителя стоит знак =
							if (line[character_index - 1] == '=')
							{
								// выполнение умножения
								x_n = x_n * double.Parse(line.Substring(character_number + 1, character_index - character_number - 1));
								line = line.Remove(character_number, character_index - character_number);
							}
							else
							{
								// выполнение умножения
								x_n = x_n * double.Parse(line.Substring(character_number + 1, character_index - character_number - 1));
								line = line.Remove(character_number, character_index - character_number);
						    }
						}
						else
						{
							// выполнение умножения
							x_n = x_n * double.Parse(line.Substring(character_number + 1, character_index - character_number - 1));
							line = line.Remove(character_number, character_index - character_number);
						}
					}
					// если после числа стоит знак /
					if (line[character_number] == '/')
					{
						for (character_index = character_number + 1; character_index < line.Length; character_index++)
						{
							if (line[character_index] == '+') break;
							if (line[character_index] == '-') break;
							if (line[character_index] == '=') break;
							if (line[character_index] == '*') break;
							if (line[character_index] == '/') break;
						}
						// если делитель - последнее число в строке
						if (character_index == line.Length)
						{
							// если после делителя стоит знак =
							if (line[character_index - 1] == '=')
							{
								// выполнение деления
								x_n = x_n / double.Parse(line.Substring(character_number + 1, character_index - character_number - 1));
								line = line.Remove(character_number, character_index - character_number);
							}
							else
							{
								// выполнение деления
								x_n = x_n / double.Parse(line.Substring(character_number + 1, character_index - character_number - 1));
								line = line.Remove(character_number, character_index - character_number);
							}
						}
						else
						{
							// выполнение деления
							x_n = x_n / double.Parse(line.Substring(character_number + 1, character_index - character_number - 1));
							line = line.Remove(character_number, character_index - character_number);
						}
					}
				}
				//if (line[character_number] == '0' && line[character_number - 1] == '=') x_n = 0;
				// если число стоит после знака =
				if (equally == 0) x_n = - x_n;
				// если перед строкой числом в строке стоит знак =
				if (line[character_number - 1] == '=') 
				{
					line = line.Remove(0, character_number);
				}
				else
				{
					// если число стоит на последнем месте в строке
					if (character_number == line.Length)
					{
						line = line.Remove(equally + 1, character_number - 1);
					}
					else
					{
						line = line.Remove(equally + 1, character_number);
					}
				}
				// к свободнуму члену прибавляется текущая переменная
				resultoffunction = resultoffunction + x_n;
				x_n = 0;
			}
			return resultoffunction;
		}
		// обнуление переменных
		public void zeroing_variables()
		{
			for(int counter = 0; counter <= 9; counter++)
			{
				coefficients[counter] = 0;
				negative_coefficients[counter] = 0;
			}
			index = 0;
 			x_n = 0;
 			multiplier_degree = 0;
 			degree = 0;
 			current_degree = 0;
 			multiplier = 0;
 			character = 0;
 			number_pos = 0;
 			character_number = 0;
 			character_index = 0;
 			line_of_output = "";
 			max_degree = 0;
 			max_negative_degree = 0;
		}
	    // проверка введенной строки
		public bool checking_the_entered_line()
		{
			// проверка на наличие знака =
			if (equation.IndexOf('=') == -1)
			{
				MessageBox.Show("Уравнение введено неверно!","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
				textBox2.AppendText("Причина ошибки - в введенной строке отсутствует знак =");
				return true;
			}
			// проверка на наличие только одной переменной
			for (char count = 'a'; count <= 'z'; count++)
				{
					if (equation.IndexOf(count) != -1 && count != 'x')
					{
						MessageBox.Show("Уравнение введено неверно!","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
						textBox2.AppendText("Причина ошибки - в введенной строке больше одной переменной");
						return true;
					}
				}
			// проверка на наличие переменной
			if (equation.IndexOf('x') == -1)
			{
				MessageBox.Show("Уравнение введено неверно!","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
				textBox2.AppendText("Причина ошибки - введенная строка не является уравнением (отсутсвует переменная)");
				return true;
			}
			// проверка степени переменной
			for (int count = 10; count < 100; count++)
			{
				if (equation.IndexOf("x^" + count) != - 1)
				{
					textBox2.Clear();
                	MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                	textBox2.AppendText("Причина ошибки - введенная строка содержит переменную в степени больше 9");
                	return true;
				}
			}
			if (equation.IndexOf('E') != -1 || equation.IndexOf("бесконечность") != -1)
			{
				MessageBox.Show("Уравнение введено неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBox2.Clear();
				textBox2.AppendText("Причина ошибки - введенно слишком большое число");
				return true;
			}
			return false;
		}
		// замена произвольной буквенной переменной на х
		public void variable_definition()
		{
			// замена русских букв х на латинские x
			equation = equation.Replace('х', 'x');
			// если переменная обозначена не символом 'x'
			if (equation.IndexOf('x') == -1)
			{
			// замена произвольной буквенной переменной на x
			for (char count = 'a'; count <= 'z'; count++)
				{
					if (equation.IndexOf(count) != -1 && count != 'x')
					{
						equation = equation.Replace(count, 'x');
						break;
					}
				}
			}
		}
		// возведение чисела в степень
		public void the_construction_of_the_numbers_in_the_degree()
		{
			// определение позиции символа ^ в уравнении
			for (number = equation.Length - 2; number >= 0; number--)
			{
				// если в степень возводится число
				if (equation[number] == '^' && equation[number - 1] != 'x' && equation[number - 1] != ')')
				 {
					// определение числа, возводимого в степень
				 	for (character_number = number - 1; character_number != -1; character_number--)
				 	{
				 		if (equation[character_number] == '+') break;
				 		if (equation[character_number] == '=') break;
				 		if (equation[character_number] == '-') break;
				 		if (equation[character_number] == '*') break;
				 		if (equation[character_number] == '/') break;
				 		if (equation[character_number] == '^') break;
				 		if (equation[character_number] == 'x') break;
				 		if (equation[character_number] == '(') break;
				 		if (equation[character_number] == ')') break;
				 	}
				 	// определение степени, в которую возводят число
				 	for (character_index = number + 1; character_index != equation.Length; character_index++)
				 	{
				 		if (equation[character_index] == '+') break;
				 		if (equation[character_index] == '=') break;
				 		if (equation[character_index] == '-') break;
				 		if (equation[character_index] == '*') break;
				 		if (equation[character_index] == '/') break;
				 		if (equation[character_index] == '^') break;
				 		if (equation[character_index] == 'x') break;
				 		if (equation[character_index] == '(') break;
				 		if (equation[character_index] == ')') break;
				 	}
				 	// переменной присваивается значение числа, возводимого в степень
				 	n1 = Double.Parse(equation.Substring(character_number + 1, number - character_number - 1));
				 	// переменной присваивается значение степени, в которую возводят число
                    n2 = Double.Parse(equation.Substring(number + 1, character_index - 1 - number));
                    // в сторку вставляется значение числа, возведенного в степень
                    equation = equation.Insert(character_index, (Math.Pow(n1, n2)).ToString());
                    equation = equation.Remove(character_number + 1, character_index - character_number - 1);
                    // обнуление переменных
                    character_number = 0;
                    character_index = 0;
				 }
			}
		}
		// решение линейного уравнения
		public void solution_of_equations_of_the_first_degree()
		{
			textBox2.AppendText("\r\n" + "Переместим свободный член в правую часть уравнения: " + coefficients[1] + "x = " + coefficients[0] * -1);
 			textBox2.AppendText("\r\n" + "Найдем решение уравнения поделив свободный член на коэффициент при х. х = " + -coefficients[0] + " / " + coefficients[1] + " = " + (coefficients[0] / - coefficients[1]));
 			textBox2.AppendText("\r\n" + "Ответ: " + (coefficients[0] / - coefficients[1]));
		}
		// решение квадратного уравнения
		public void solution_to_the_quadratic_equation()
		{
			double discriminant,  first_root, second_root;
			// нахождение дискриминанта
			discriminant = coefficients[1] * coefficients[1] + (-4 * coefficients[2] * coefficients[0]);
			textBox2.AppendText("\r\n" + "Коэффициенты уравнения: a=" + coefficients[2] + "; b=" + coefficients[1] + "; c=" + coefficients[0] + ";");
  			textBox2.AppendText("\r\n" + "Находим дискриминант по формуле D=b*b - 4*a*c. Он равен " + discriminant);
  			// если дискриминант < 0
  			if (discriminant < 0)
  			{
      			textBox2.AppendText("\r\n" + "Дискриминант < 0, значит уравнение не имеет решений на множестве действительных чисел.");
      			textBox2.AppendText("\r\n" + "Находим решения в комплексных числах:");
      			textBox2.AppendText("\r\n" + "Первый корень уравнения = (-b+vD)/(2*a) = (" + -coefficients[1] + " + " + Math.Sqrt(-discriminant) + "i ) / (  2 * " + coefficients[2] + " )" + " = " + (-coefficients[1]) / (2 * coefficients[2]) + " + " + Math.Sqrt(-discriminant) + "i");
      			textBox2.AppendText("\r\n" + "Второй корень уравнения = (-b-vD)/(2*a) = (" + -coefficients[1] + " - " + Math.Sqrt(-discriminant) + "i ) / (  2 * " + coefficients[2] + " )" + " = " + (-coefficients[1]) / (2 * coefficients[2]) + " - " + Math.Sqrt(-discriminant) + "i");
      			// если квадратное уравнение не было получилучено в ходе приведения
      			if (coefficients[3] == 0)
      			{
      				textBox2.AppendText("\r\n" + "Ответы: x1 = " + (-coefficients[1]) / (2 * coefficients[2]) + " + " + Math.Sqrt(-discriminant) + "i;" + "x2 = " + (-coefficients[1]) / (2 * coefficients[2]) + " - " + Math.Sqrt(-discriminant) + "i;");
      				return;
      			}
      			else
      			{
      				textBox2.AppendText("\r\n" + "Третий корень = 0");
      				textBox2.AppendText("\r\n" + "Ответы: x1 = " + (-coefficients[1]) / (2 * coefficients[2]) + " + " + Math.Sqrt(-discriminant) + "i;" + "x2 = " + (-coefficients[1]) / (2 * coefficients[2]) + " - " + Math.Sqrt(-discriminant) + "i;");
      				return;
      			}
  			}
  			// дискриминант = 0
  			if (discriminant == 0)
  			{
  				first_root = (- coefficients[1]) / (2 * coefficients[2]);
  				textBox2.AppendText("\r\n" + "Дискриминант = 0, значит уравнение имеет одно решение. Оно находится по формуле: x= -b /(2*a)");
  				textBox2.AppendText("\r\n" + "Корень уравнения = " + first_root);
  				// если квадратное уравнение не было получилучено в ходе приведения
  				if (coefficients[3] == 0)
  				{
  					textBox2.AppendText("\r\n" + "Ответ: " + first_root);
  				}
  				else
  				{
  					textBox2.AppendText("\r\n" + "Второй корень = 0");
  					textBox2.AppendText("\r\n" + "Ответы: x1 = " + first_root + "; x2 = 0");
  				}
  			}
			// дискриминант > 0
            else
            {
            	first_root = (-coefficients[1] + Math.Sqrt(discriminant)) / (2 * coefficients[2]);
            	second_root = (-coefficients[1] - Math.Sqrt(discriminant)) / (2 * coefficients[2]);
            	textBox2.AppendText("\r\n" + "Дискриминант > 0, значит уравнение имеет два решения. Они находятся по формуле: x1 = (-b+vD)/(2*a) и x2 = (-b-vD)/(2*a)");
            	textBox2.AppendText("\r\n" + "Первый корень уравнения = " + first_root + " . " + "Второй корень уравнения = " + second_root);
            	// если квадратное уравнение не было получилучено в ходе приведения
            	if (coefficients[3] == 0)
            	{
            		textBox2.AppendText("\r\n" + "Ответы: " + first_root + " и " + second_root);
            	}
            	else
            	{
            		textBox2.AppendText("\r\n" + "Третий корень = 0");
            		textBox2.AppendText("\r\n" + "Ответы: x1 = " + first_root + "; x2 = " + second_root + "; x3 = 0");
            	}
            }
 		}
		// решение кубического уравнения
		public void solving_cubic_equations()
		{
			double Q, R, x1, x2, x3, S, p, xx;
			textBox2.AppendText("\r\n" + "Коэффициенты уравнения равны: a=" + coefficients[3] + "; b=" + coefficients[2] + "; c=" + coefficients[1] + "; d=" + coefficients[0] + ";");
			// решение уравнения вида ax^3=d
  			if (coefficients[1] == 0 && coefficients[2] == 0)
  			{
  				xx = -coefficients[0] / coefficients[3];
  				x1 = Math.Sign(xx) * Math.Pow(Math.Abs(xx), 1/3);
  				textBox2.AppendText("\r\n" + "Коэффициенты b и c = 0.");
  				textBox2.AppendText("\r\n" + "Единственный вещественный корень уравнения равен корню 3 степени из d / a;");
  				textBox2.AppendText("\r\n" + "x = " + x1);
  				textBox2.AppendText("\r\n" + "Ответ: х = " + x1);
  				return;
  			}
  			// решение уравнения вида ax^3+bx^2+cx=0
  			if (coefficients[0] == 0)
  			{
  				coefficients[0] = coefficients[1];
                coefficients[1] = coefficients[2];
                coefficients[2] = coefficients[3];
                line_of_output = "(";
                for (degree = 2; degree >= 0; degree--)
                {
                	if (coefficients[degree] != 0)
                	{
                		if  (line_of_output != "(") 
                		{
                			if (coefficients[degree] > 0) 
                			{
                				line_of_output = line_of_output + "+";
                			}
                		}
                		if (degree == 0) 
                		{
                			line_of_output = line_of_output + coefficients[degree];
                		}
                		if (degree == 1) 
                		{
                			line_of_output = line_of_output + coefficients[degree] + "x";
                		}
                		if (degree > 1) 
                		{
                			line_of_output = line_of_output + coefficients[degree] + "x^" + degree;
                		}
                	}
                }
                textBox2.AppendText("\r\n" + "Свободный член равен 0, вынесем х за скобки:" + line_of_output + ")*x=0");
                textBox2.AppendText("\r\n" + "Произведение равно нулю если хоть один из множителей равен нулю, а остальные при этом не теряют смысл.");
                textBox2.AppendText("\r\n" + "Первые два корня уравнения найдем, приравняв выражение в скобках к нулю.");
                textBox2.AppendText("\r\n" + "Решим полученное квадратное уравнение через дискриминант.");
                solution_to_the_quadratic_equation();
                return;	
  			}
  			coefficients[2] = coefficients[2] / coefficients[3];
  			coefficients[1] = coefficients[1] / coefficients[3];
  			coefficients[0] = coefficients[0] / coefficients[3];
  			textBox2.AppendText("\r\n" + "Приведем уравнение к виду х^3 + b/a*x^2 + c/a*x + k = 0, поделив коэффициенты b,c и d на a.");
  			textBox2.AppendText("\r\n" + "Новые коэффициенты равны: b=" + coefficients[2] + "; c=" + coefficients[1] + "; d=" + coefficients[0] + ";");
 			Q = (coefficients[2] * coefficients[2] - 3 * coefficients[1]) / 9;
  			R = (2 * coefficients[2] * coefficients[2] * coefficients[2] - 9 * coefficients[2] * coefficients[1] + 27 * coefficients[0]) / 54;
  			S = Q * Q * Q - R * R;
  			textBox2.AppendText("\r\n" + "Вычислим Q, R и S.");
  			textBox2.AppendText("\r\n" + "Q = (b^2-3*c)/9 = " + Q + ".");
  			textBox2.AppendText("\r\n" + "R = (2*b^3-9*b*c+27*c)/54 = " + R + ".");
  			textBox2.AppendText("\r\n" + "S = Q^3-R^2 = " + S + ".");
  			if (S > 0)
  			{
  				p = (Math.Acos(R / Math.Pow(Q, 3 / 2))) / 3;
   				textBox2.AppendText("\r\n" + "S > 0. Уравнение имеет три вещественных корня.");
   				textBox2.AppendText("\r\n" + "Найдем p = (arccos(R/Q^(3/2)))/3 = " + p);
   				x1 = - 2 * Math.Pow(Q, 1f/2f) * Math.Cos(p) - coefficients[2] / 3;
   				x2 = - 2 * Math.Pow(Q, 1f/2f) * Math.Cos(p + 2 * Math.PI / 3) - coefficients[2] / 3;
   				x3 = - 2 * Math.Pow(Q, 1f/2f) * Math.Cos(p - 2 * Math.PI / 3) - coefficients[2] / 3;
                x1 = Math.Round(x1, 10);
                x2 = Math.Round(x2, 10);
                x3 = Math.Round(x3, 10);
                textBox2.AppendText("\r\n" + "Корни уравнения находятся по формуле:");
   				textBox2.AppendText("\r\n" + "x1= - 2 * (Q)^(1/2) * cos(p) - b/3 = " + x1);
   				textBox2.AppendText("\r\n" + "x2= - 2 * (Q)^(1/2) * cos(p+2*Pi/3) - b/3 = " + x2);
   				textBox2.AppendText("\r\n" + "x3= - 2 * (Q)^(1/2) * cos(p-2*Pi/3) - b/3 = " + x3);
   				textBox2.AppendText("\r\n" + "Ответы: x1 = " + x1 + "; x2 = " + x2 + "; x3 = " + x3);
  			}
  			if (S < 0)
  			{
  				if (Q > 0)
  				{
  					p = arch((Math.Abs(R)) / Math.Pow(Math.Abs(Q), 3/2)) / 3;
               		textBox2.AppendText("\r\n" + "'S < 0 и Q > 0. Уравнение имеет один вещественный и два мнимых корня.");
               		textBox2.AppendText("\r\n" + "Найдем p =  ln(|R|/|Q|^(3/2) + ((|R|/|Q|^(3/2))^2-1)^(1/2)) = " + p);
               		x1 = - 2 * Math.Sign(R) * Math.Pow(Math.Abs(Q), 1f/2f) * ((Math.Exp(p) + Math.Exp(-p)) / 2) - coefficients[2] / 3;
                    x1 = Math.Round(x1, 10);
                    textBox2.AppendText("\r\n" + "Найдем вещественный корень по формуле:");
               		textBox2.AppendText("\r\n" + "х1 = -2*(знак R)*|Q|^(1/2)*((e^p + e^(-p))/2) - b/3 = " + x1);
               		x2 = Math.Sign(R) * Math.Pow(Math.Abs(Q), 1f/2f) * ((Math.Exp(p) + Math.Exp(-p))/2) - coefficients[2] / 3;
               		x3 = Math.Pow(3 * Math.Abs(Q), 1f/2f) * ((Math.Exp(p) - Math.Exp(-p))/2);
                    x2 = Math.Round(x2, 10);
                    x3 = Math.Round(x3, 10);
                    textBox2.AppendText("\r\n" + "Найдем первый мнимый корень по формуле:");
               		textBox2.AppendText("\r\n" + "х2 = (знак R)*|Q|^(1/2)*((e^p + e^(-p))/2) - b/3 + ((3*|Q|)^(1/2) * ((e^p - e^(-p))/2))i = " + x2 + "+" + x3 + "i");
               		textBox2.AppendText("\r\n" + "Найдем второй мнимый корень по формуле:");
               		textBox2.AppendText("\r\n" + "х3 = (знак R)*|Q|^(1/2)*((e^p + e^(-p))/2) - b/3 - ((3*|Q|)^(1/2) * ((e^p - e^(-p))/2))i = " + x2 + "-" + x3 + "i");
               		textBox2.AppendText("\r\n" + "Ответы: x1 = " + x1 + "; x2 = " + x2 + "+" + x3 + "i; x3 = " + x2 + "-" + x3 + "i");
  				} 
             	else
             	{
             		p = arsh((Math.Abs(R)) / Math.Pow(Math.Abs(Q), 3f/2f)) / 3f;
               		textBox2.AppendText("\r\n" + "S < 0 и Q < 0. Уравнение имеет один вещественный и два мнимых корня.");
               		textBox2.AppendText("\r\n" + "Найдем p =  ln(|R|/|Q|^(3/2) + ((|R|/|Q|^(3/2))^2+1)^(1/2)) = " + p);
               		x1 = - 2 * Math.Sign(R) * Math.Pow(Math.Abs(Q), 1f/2f) * ((Math.Exp(p) - Math.Exp(-p)) / 2f) - coefficients[2] / 3f;
               		textBox2.AppendText("\r\n" + "Найдем вещественный корень по формуле:");
               		textBox2.AppendText("\r\n" + "х1 = -2*(знак R)*|Q|^(1/2)*((e^p - e^(-p))/2) - b/3 = " + x1);
                    x2 = Math.Sign(R) * Math.Pow(Math.Abs(Q), 1f / 2f) * ((Math.Exp(p) - Math.Exp(-p)) / 2f) - coefficients[2] / 3f;
               		x3 = Math.Pow(3 * Math.Abs(Q), 1f/2f) * ((Math.Exp(p) + Math.Exp(-p)) / 2f);
                    x1 = Math.Round(x1, 10);
                    x2 = Math.Round(x2, 10);
                    x3 = Math.Round(x3, 10);
                    textBox2.AppendText("\r\n" + "Найдем первый мнимый корень по формуле:");
               		textBox2.AppendText("\r\n" + "х2 = (знак R)*|Q|^(1/2)*((e^p - e^(-p))/2) - b/3 + ((3*|Q|)^(1/2) * ((e^p + e^(-p))/2))i = " + x2 + "+" + x3 + "i");
               		textBox2.AppendText("\r\n" + "Найдем второй мнимый корень по формуле:");
               		textBox2.AppendText("\r\n" + "х3 = (знак R)*|Q|^(1/2)*((e^p - e^(-p))/2) - b/3 - ((3*|Q|)^(1/2) * ((e^p + e^(-p))/2))i = " + x2 + "-" + x3 + "i");
 					textBox2.AppendText("\r\n" + "Ответы: x1 = " + x1 + "; x2 = " + x2 + "+" + x3 + "i; x3 = " + x2 + "-" + x3 + "i");	
             	}			
  			}
  			if (S == 0)
  			{
  				textBox2.AppendText("\r\n" + "S = 0. Уравнение имеет два вещественных корня. Найдем иx:");
  				x1 = - 2 * Math.Pow(R,1f/3f) - coefficients[2] / 3;
  				x2 = Math.Pow(R, 1f/3f) - coefficients[2] / 3;
                x1 = Math.Round(x1, 10);
                x2 = Math.Round(x2, 10);
                textBox2.AppendText("\r\n" + "x1= -2 * R^(1/3) - b/3 = " + x1);
  				textBox2.AppendText("\r\n" + "x2= R^(1/3) - b/3 = " + x2);
  				textBox2.AppendText("\r\n" + "Ответы: x1 = " + x1 + "; x2 = " + x2);
  			}
		}
		// нахождение arch от числа
		public double arch(double x)
		{
			double Result = Math.Log(x + Math.Sqrt(x * x - 1));
			return Result;
		}
		// нахождение arsh от числа
		public double arsh(double x)
		{
			double Result = Math.Log(x + Math.Sqrt(x * x + 1));
			return Result;
		}
		// решение уравнений 4-9 степени по теореме Горнера
		public void solution_on_Gorner_theorem()
		{
			int current_degree, root, j;
			double sum1, sum2;
			double[] current_coefficients = new double[10];
			string line, line_two = "";
			line_of_output = "";
 			current_degree = degree;
 			j = 1;
 			for (root = degree; root >= 0; root--)
 			{
 				current_coefficients[j] = coefficients[root];
 				j = j + 1;	
 			}
 			while (current_degree > 3)
 			{
  				line = "";
  				root = 0;
  				while (root > Math.Abs(current_coefficients[current_degree + 1]))
  				{
   					sum1 = 0;
   					sum2 = 0;
   					// найдем корень уравнения
   					for (j = 1; j <= current_degree; j++)
   					{
   						sum1 = sum1 + current_coefficients[j] * Math.Pow(root, current_degree - (j - 1));
   						sum2 = sum2 + current_coefficients[j] * Math.Pow(- root, current_degree - (j - 1));
   					}
   					sum1 = sum1 + current_coefficients[current_degree + 1];
   					sum2 = sum2 + current_coefficients[current_degree + 1];
   					if (sum1 == 0) break;
   					if (sum2 == 0) 
   					{
   						root = - root; 
   						break;
   					}
   					root = root + 1;
  				}
  				if (root > 0) line_two = line_two + "(x-" + root + ")";
  				if (root < 0) line_two = line_two + "(x+" + Math.Abs(root) + ")";
 				// если в уравнении отсутствует свободный член
 				if (root == 0)
 				{
           			//выносим x за скобки
           			line_two = line_two + "(x)";
           			for (j = 1; j <= current_degree; j++)
           			{
           				if (j == current_degree)
           				{
           					if (current_coefficients[j] > 0) line = line + "+" + current_coefficients[j];
           					if (current_coefficients[j] < 0) line = line + current_coefficients[j];
           					continue;
           				}
           				if (j == current_degree - 1)
           				{
           					if (current_coefficients[j] > 0) line = line + "+" + current_coefficients[j] + "x";
           					if (current_coefficients[j] < 0) line = line + current_coefficients[j] + "x";
           					continue;
           				}
           				if (current_coefficients[j] > 0) line = line + "+" + current_coefficients[j] + "x^" + (current_degree - j);
           				if (current_coefficients[j] < 0) line = line + current_coefficients[j] + "x^" + (current_degree - j);
           			}
          		textBox2.AppendText("\r\n" + "Свободный член = 0. Вынесем х за скобки.");
          		textBox2.AppendText("\r\n" + "Получается уравнение: " + line_two + "(" + line + ")=0");
 				}
          		else
          		{
          			// выносим за скобки х - полученный корень, используя схему Горнера
           			textBox2.AppendText("\r\n" + "Методом подбора найдем один корень данного уравнения. Обычно он является делителем свободного члена");
           			textBox2.AppendText("\r\n" + "Он равен " + root);
           			current_coefficients[1] = root;
           			line = root + "x^" + (current_degree - 1);
           			for (j = 2; j <= current_degree; j++)
           			{
            			current_coefficients[j] = current_coefficients[j - 1] * root + current_coefficients[j];
            			if (j == current_degree)
            			{
            				if (current_coefficients[j] > 0) line = line + "+" + current_coefficients[j];
            				if (current_coefficients[j] < 0) line = line + current_coefficients[j];
            				continue;
            			}
            			if (j == current_degree - 1)
            			{
            				if (current_coefficients[j] > 0) line = line + "+" + current_coefficients[j] + "x";
            				if (current_coefficients[j] < 0) line = line + current_coefficients[j] + "x";
            				continue;
            			}
            			if (current_coefficients[j] > 0) line = line + "+" + current_coefficients[j] + "x^" + (current_degree - j);
            			if (current_coefficients[j] < 0) line = line + current_coefficients[j] + "x^" + (current_degree - j);
           			}
          			textBox2.AppendText("\r\n" + "Вынесем (используя схему Горнера) выражение (х - найденный корень) за скобки.");
          			textBox2.AppendText("\r\n" + "Получается уравнение: " + line_two + "(" + line + ")=0");
  				}          
 				current_degree = current_degree - 1;
 				// если в скобках получилось уравнение 3 степени
 				if (current_degree == 3)
 				{
 					line_of_output = line_of_output + root + ".";
 					coefficients[0] = current_coefficients[4];
 					coefficients[1] = current_coefficients[3];
 					coefficients[2] = current_coefficients[2];
 					coefficients[3] = current_coefficients[1];
 					if (coefficients[3] == 0 && coefficients[2] == 0) 
 					{
 						textBox2.AppendText("\r\n" + "Крайний правый множитель является линейным уравнением . Решим его"); 
 						solution_of_equations_of_the_first_degree();
 					}
 					if (coefficients[3] == 0 && coefficients[2] != 0) 
 					{
 					textBox2.AppendText("\r\n" + "Крайний правый множитель является уравнением 2 степени. Решим его через дискриминант"); 
 					solution_to_the_quadratic_equation();
 					}
 					if (coefficients[3] != 0) 
 					{
 						textBox2.AppendText("\r\n" + "Крайний правый множитель является уравнением 3 степени. Решим его по тригонометрической теореме Виета"); 
 						solving_cubic_equations();
 					}
 					textBox2.AppendText("\r\n" + "Ответы, полученные по схеме Горнера: " + line_of_output);
 					line_of_output = "";
 				}
 				else
 				{
 					// в скобках получилось уравнение степени 4 и выше, продолжим раскладывать его на множители
                	line_of_output = line_of_output + root + "; ";
                	textBox2.AppendText("\r\n" + "Крайний правый множитель является уравнением степени n > 3. Продолжим раскладывать уравнение на множители");
 				}
 			}
		}
		// замена символов, обозначающих дробь, в веденном числе на системные
		void TextBox1KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '.' || e.KeyChar == ',')
			{
				e.KeyChar = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
			}
		}
		// обработка нажатия на кнопку "Enter"
		void Button1KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				// решение уравнения
				Button1Click(sender, e);
			}
		}
		// обработка нажатия на кнопку "Enter"
		void MainFormKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				// решение уравнения
				Button1Click(sender, e);
			}
		}

	}
}