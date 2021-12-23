using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace ConsultaBancoDeDados
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Abrindo a connection string!");

            var connectionString = @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XE)));User Id=AppAcademy;Password=AppAcademy;";

            var aluno = new Aluno
            {
                Id = Guid.NewGuid(),
                Nome = "Italo",
                Idade = 4,
                Curso = "Matematica"
            };

            AdicionarAlunos(connectionString, aluno);

            var aluno2 = new Aluno
            {
                Id = Guid.Parse("dfeea2f7-d469-49e0-a722-7627af3fde1d"),
                Nome = "joaozinho",
                Idade = 9,
                Curso = "Historia"
            };

            AtualizarAlunos(connectionString, aluno2);

            LerAlunos(connectionString);

            //DeletarAlunos(connectionString, aluno2);
            //Console.WriteLine("Vai deletar");
            //LerAlunos(connectionString);

        }

        private static void AdicionarAlunos(string connectionString, Aluno aluno)
        {
            
            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();

                using var cmd = new OracleCommand(
                    @"INSERT INTO APPACADEMY.ALUNOS
                        (ID, NOME, IDADE, CURSO)
                      VALUES(:Id,:Nome,:Idade,:Curso)", conn);

                cmd.Parameters.Add(new OracleParameter("Id",aluno.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome",aluno.Nome));
                cmd.Parameters.Add(new OracleParameter("Idade",aluno.Idade));
                cmd.Parameters.Add(new OracleParameter("Curso",aluno.Curso));

                var rows = cmd.ExecuteNonQuery();
                
            }

        }

        private static void AtualizarAlunos(string connectionString, Aluno aluno)
        {
            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();

                using var cmd = new OracleCommand(
                    @"UPDATE APPACADEMY.ALUNOS
                       SET Id = :Id,
                           Nome = :Nome,
                           Idade = :Idade, 
                           Curso = :Curso
                       where Id = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", aluno.Id.ToString()));
                cmd.Parameters.Add(new OracleParameter("Nome", aluno.Nome));
                cmd.Parameters.Add(new OracleParameter("Idade", aluno.Idade));
                cmd.Parameters.Add(new OracleParameter("Curso", aluno.Curso));

                var rows = cmd.ExecuteNonQuery();

            }

        }

        private static void DeletarAlunos(string connectionString, Aluno aluno)
        {
            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();

                using var cmd = new OracleCommand(
                    @"DELETE APPACADEMY.ALUNOS                       
                       where Id = :Id", conn);

                cmd.Parameters.Add(new OracleParameter("Id", aluno.Id.ToString()));

                var rows = cmd.ExecuteNonQuery();

            }

        }

        private static void LerAlunos(string connectionString)
        {
            try
            {
                var alunos = new List<Aluno>();

                //Quando usar o using a desalocação ocorre quando usa-se chave, ao final da chave ele faz a desalocação se não usar chave ele desaloca somente quando
                //finalizar o bloco ao qual o comando está inserido
                using (var conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    /*
                    using var cmd = new OracleCommand("select sysdate from dual", conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        //por boa prática, sempre usar while, mesmo que a agente saiba que vai retornar um único dado.
                        while (reader.Read())
                        {

                            //Se eu quiser uma data precisa fazer um parse ou convert. Sempre vamos ter que fazer o parse pro tipo de dado que queremos.
                            var data = reader["sysdate"].ToString();
                            var data2 = Convert.ToDateTime(reader["sysdate"]);

                            Console.WriteLine("A data do servidor é " + data + " --- " + data2);
                        }
                    }
                    */

                    //Nunca colocar os parametros direto na montagem do comando, pois isso é sql injection. Sempre adicionar através de OracleParameter ou até, posso
                    //usar um string builder para montar o filtro de um select
                    using var cmd = new OracleCommand("select * from alunos", conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        //por boa prática, sempre usar while, mesmo que a agente saiba que vai retornar um único dado.
                        while (reader.Read())
                        {
                            var aluno = new Aluno
                            {
                                Id = Guid.Parse(reader["id"].ToString()),
                                Nome = reader["nome"].ToString(),
                                Idade = Convert.ToInt32(reader["idade"]),
                                Curso = reader["nome"].ToString()
                            };

                            alunos.Add(aluno);

                        }
                    }

                    foreach (var aluno in alunos)
                    {
                        Console.WriteLine(aluno.ToString());
                        Console.WriteLine();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }

    public class Aluno
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public int Idade { get; set; }
        public string Curso { get; set; }

        public override string ToString()
        {
            return @$"ID: {Id}
                   Nome: {Nome}
                   Idade: {Idade}
                   Curso: {Curso}";
        }
    }
}
