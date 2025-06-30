using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ApiExamen.Models;
using ApiExamen.Interfaces;

namespace ApiExamen.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly string _connectionString;

        public EmpleadoService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task CrearEmpleadoConUsuario(EmpleadoUsuarioDTO dto)
        {
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var tran = con.BeginTransaction();

            try
            {
                // Obtener nuevo codigoEmpleado
                var nuevoCodigo = await con.ExecuteScalarAsync<int>(
                    "SELECT ISNULL(MAX(codigoEmpleado), 0) + 1 FROM Empleados", transaction: tran);

                // Insertar empleado
                var insertEmpleado = @"EXEC spInsertarEmpleado
            @codigoEmpleado, @nombre, @apellidoPaterno, @apellidoMaterno, @fechaNacimiento, @fechaInicioContrato, @idPuesto";

                await con.ExecuteAsync(insertEmpleado, new
                {
                    codigoEmpleado = nuevoCodigo,
                    nombre = dto.Nombre,
                    apellidoPaterno = dto.ApellidoPaterno,
                    apellidoMaterno = dto.ApellidoMaterno,
                    fechaNacimiento = dto.FechaNacimiento,
                    fechaInicioContrato = dto.FechaInicioContrato,
                    idPuesto = dto.IdPuesto
                }, transaction: tran);

                // Insertar usuario para login (en texto plano, igual que tus inserts manuales)
                var insertUsuario = @"INSERT INTO Usuarios (usuario, contrasena, rol, codigoEmpleado)
                              VALUES (@usuario, @contrasena, 'Empleado', @codigoEmpleado)";

                await con.ExecuteAsync(insertUsuario, new
                {
                    usuario = dto.Usuario,
                    contrasena = dto.Contrasena,
                    codigoEmpleado = nuevoCodigo
                }, transaction: tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Empleado>> Consultar()
        {
            using var con = new SqlConnection(_connectionString);
            return await con.QueryAsync<Empleado>("spConsultarEmpleados", commandType: CommandType.StoredProcedure);
        }

        public async Task Crear(Empleado e)
        {
            using var con = new SqlConnection(_connectionString);

            // Obtener el siguiente códigoEmpleado iniciando desde 1
            var nuevoCodigo = await con.ExecuteScalarAsync<int>(
                "SELECT ISNULL(MAX(codigoEmpleado), 0) + 1 FROM Empleados");

            e.codigoEmpleado = nuevoCodigo;

            // Ejecutar el procedimiento almacenado
            await con.ExecuteAsync("spInsertarEmpleado", new
            {
                e.codigoEmpleado,
                e.nombre,
                e.apellidoPaterno,
                e.apellidoMaterno,
                e.fechaNacimiento,
                e.fechaInicioContrato,
                e.idPuesto
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task Actualizar(Empleado e)
        {
            using var con = new SqlConnection(_connectionString);
            await con.ExecuteAsync("spActualizarEmpleado", e, commandType: CommandType.StoredProcedure);
        }

        public async Task Eliminar(int codigoEmpleado)
        {
            using var con = new SqlConnection(_connectionString);

            // Elimina el usuario asociado primero
            await con.ExecuteAsync("DELETE FROM Usuarios WHERE codigoEmpleado = @codigoEmpleado", new { codigoEmpleado });

            await con.ExecuteAsync("spEliminarEmpleado", new { codigoEmpleado }, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<Empleado>> ObtenerTodos()
        {
            using var con = new SqlConnection(_connectionString);
            var result = await con.QueryAsync<Empleado>("spConsultarEmpleados", commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<Empleado?> ObtenerPorCodigo(int codigoEmpleado)
        {
            using var con = new SqlConnection(_connectionString);
            return await con.QueryFirstOrDefaultAsync<Empleado>(
                "SELECT * FROM Empleados WHERE codigoEmpleado = @codigoEmpleado", 
                new { codigoEmpleado });
        }

        public async Task<EmpleadoUsuarioDTO?> ObtenerEmpleadoConCredenciales(string usuario, string contrasena)
        {
            using var con = new SqlConnection(_connectionString);
            var query = @"
                SELECT e.*, u.usuario, u.contrasena
                FROM Empleados e
                JOIN Usuarios u ON e.codigoEmpleado = u.codigoEmpleado
                WHERE u.usuario = @usuario AND u.contrasena = @contrasena";
            
            return await con.QueryFirstOrDefaultAsync<EmpleadoUsuarioDTO>(query, new { usuario, contrasena });
        }
    }
}
