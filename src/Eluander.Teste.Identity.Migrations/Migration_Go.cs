using Eluander.Infra.Identity.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using SimpleMigrations;
using SimpleMigrations.DatabaseProvider;
using System.Reflection;

namespace Eluander.Teste.Identity.Migration
{
    [TestClass]
    public class Migration_Go
    {
        private const string cnn = "User ID=postgres;Host=127.0.0.1;Port=5432;Database=fluentnhib;Password=123;";
        private Assembly assemblyTo = typeof(_0001_aspnet_core_Identity).Assembly;

        [TestMethod]
        public void UpgradeDataBase()
        {
            // start conecx�o da db.
            using (var connection = new NpgsqlConnection(cnn))
            {
                var databaseProvider = new PostgresqlDatabaseProvider(connection);
                var migrator = new SimpleMigrator(assemblyTo, databaseProvider);

                // carregar todos os estados de vers�es da base de dados.
                migrator.Load();

                // quando existir uma vers�o j� ativa, inicie as migra��es apratir de uma vers�o especifica.
                //migrator.Baseline(10);

                // verificar se a vers�o da db atual � diferente da ultima vers�o dos arquivos de migra��es.
                if (migrator.CurrentMigration.Version != migrator.LatestMigration.Version)
                {
                    // atualizar db para ultima vers�o.
                    migrator.MigrateToLatest();

                    // atualizar db at� o arquivo de migra��o numero 11.
                    //migrator.MigrateTo(11);
                }

                Assert.IsTrue(migrator.CurrentMigration.Version == migrator.LatestMigration.Version);
            }
        }

        [TestMethod]
        public void DowngradeDataBase()
        {
            int versionTo = 0;

            using (var connection = new NpgsqlConnection(cnn))
            {
                var databaseProvider = new PostgresqlDatabaseProvider(connection);
                var migrator = new SimpleMigrator(assemblyTo, databaseProvider);

                // carregar todos os estados de vers�es da base de dados.
                migrator.Load();

                // a vers�o da db atual deve ser maior que a vers�o versionTo.
                if (migrator.CurrentMigration.Version > versionTo)
                {
                    // vers�o da db para qual deseja voltar.
                    migrator.MigrateTo(versionTo);
                }

                Assert.IsTrue(migrator.CurrentMigration.Version == migrator.LatestMigration.Version);
            }
        }
    }
}
