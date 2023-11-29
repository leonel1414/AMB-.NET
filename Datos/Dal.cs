using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Entidade;


namespace Datos
{
    public class Dal
    {
        private int identidad;
        public int Identidad
        {
            get
            {
                return identidad;
            }
            set
            {
                identidad = value;
            }
        }

        public static string SQL = string.Empty;
        public DataSet BuscarLegajo(int plegajo)
        {
            SQL = "SELECT * FROM DATOSALUMNOS WHERE LEGAJO='" + plegajo + "'";
            SqlConnection con = new SqlConnection(Conexion.sConnection);
            DataSet objDataset = new DataSet(); // es un objeto contenedor, almacena temporalmente la informacion de la base
            SqlCommand com = new SqlCommand(SQL, con); // permite ejecutar comandos (sentencias Sql)
            SqlDataAdapter da = new SqlDataAdapter();// sirve como puente entre DataSet y SQL para recuperar y guardar datos
            da.SelectCommand = com;

            try
            {
                da.Fill(objDataset); //Agrega o actualiza filas en el DataSet
            }
            catch (SqlException ex)
            {

                throw new Exception("Error en la base de datos" + ex.Message);
            }
            finally
            {
                con.Dispose();//Libera todos los recursos que usa la conexion
                com.Dispose();//Libera todos los recursos que usa los comandos/sentencias
            }
            return objDataset;//retorna informacion
        }

        public DataSet BuscarApeNom(string pApenom)
        {

            SQL = "SELECT * FROM DATOSALUMNOS WHERE APE_NOM LIKE '%' + '" + pApenom + "' + '%'";
            SqlConnection con = new SqlConnection(Conexion.sConnection);
            DataSet objDataset = new DataSet();
            SqlCommand com = new SqlCommand(SQL, con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = com;

            try
            {
                da.Fill(objDataset);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error en la base de datos" + ex.Message);
            }
            finally
            {
                con.Dispose();
                com.Dispose();
            }
            return objDataset;
        }


        public static List<Ecivil> ListarEcivil()
        {
            List<Ecivil> Lista = new List<Ecivil>();
            SQL = "SELECT DESC_ECIV, COD_ECIV FROM ECIVIL ORDER BY DESC_ECIV";
            SqlConnection con = new SqlConnection(Conexion.sConnection);
            SqlCommand com = new SqlCommand(SQL, con);

            try
            {
                con.Open();
                SqlDataReader objReader = com.ExecuteReader();

                while (objReader.Read())
                {
                    //si toma 2 argumentos, para ello necesito un constructor
                    Ecivil Item = new Ecivil(Convert.ToInt32(objReader["COD_ECIV"]), objReader["DESC_ECIV"].ToString());
                    Lista.Add(Item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
            return Lista;
        }

        public DataSet BuscarAlumnos()
        {
            SQL = "SELECT * FROM DATOSALUMNOS ORDER BY LEGAJO DESC";
            SqlConnection con = new SqlConnection(Conexion.sConnection);
            DataSet objDataset = new DataSet();
            SqlCommand com = new SqlCommand(SQL, con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = com;
            try
            {
                da.Fill(objDataset);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error en la base de datos" + ex.Message);

            }
            finally
            {
                con.Dispose();
                com.Dispose();
            }
            return objDataset;

        }

        // El objeto transaccion permite ejecutar 2 o mas comandos en el contexto de una transaccion
        // se utiliza los metodos commit() y rollback()
        public static SqlTransaction objTransaction = null;
        public int AgregarAlumno(int pLegajo, string pApenom, DateTime pFchnac, int pNrodoc, int pCodeEciv, string pTelcelu, string pEmail, string pFotojpg, string pLegajoPdf, string pObservac)
        {
            SQL = "INSERT INTO DATOSALUMNOS(LEGAJO, APE_NOM, FCH_NAC,NRODOC, COD_ECIV, TEL_CELU, E_MAIL, FOTO_JPG, LEGAJO_PDF, OBSERVAC) VALUES";
            SQL = SQL + "(@pLegajo, @pApenom, @pFchnac, @pNrodoc, @pCodEciv, @pTelcelu, @pEmail, @pFotojpg, @pLegajopdf,@pObservac)";

            SqlConnection con = new SqlConnection(Conexion.sConnection);
            SqlCommand com = new SqlCommand(SQL, con);
            SqlParameter retParam = com.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
            retParam.Direction = ParameterDirection.ReturnValue;
            com.Parameters.Add("@pLegajo", SqlDbType.Int, 1).Value = pLegajo;
            com.Parameters.Add("@pApenom", SqlDbType.VarChar, 100).Value = pApenom;
            com.Parameters.Add("@pFchnac", SqlDbType.DateTime).Value = pFchnac;
            com.Parameters.Add("@pNrodoc", SqlDbType.Int, 1).Value = pNrodoc;
            com.Parameters.Add("@pCodEciv", SqlDbType.Int, 1).Value = pCodeEciv;
            com.Parameters.Add("@pTelcelu", SqlDbType.VarChar, 15).Value = pTelcelu;
            com.Parameters.Add("@pEmail", SqlDbType.VarChar, 30).Value = pEmail;
            com.Parameters.Add("@pFotojpg", SqlDbType.VarChar, 16).Value = pFotojpg;
            com.Parameters.Add("@pLegajopdf", SqlDbType.VarChar, 16).Value = pLegajoPdf;
            com.Parameters.Add("@pObservac", SqlDbType.VarChar, 16).Value = pObservac;
            try
            {
                con.Open();

                objTransaction = con.BeginTransaction();
                com.Transaction = objTransaction;
                com.ExecuteNonQuery();
                identidad = Convert.ToInt32(retParam.Value);
                //Commit: confirma los comando que se han ejecutado en el contexto de la transaccion
                objTransaction.Commit();

            }
            catch (SqlException ex)
            {
                //Rollback: cancela los comandos que se han ejecutado en el contexto de la transaccion
                objTransaction.Rollback();
                throw new Exception("Error en Base de Datos" + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Dispose();
                com.Dispose();
            }
            return identidad;
        }

        public int ModificarAlumno(int pLegajo, string pApenom, DateTime pFchnac, int pNrodoc, int pCodEciv, string pTelcelu, string pEmail, string pFotojpg, string pLegajopdf, string pObservac)
        {
            SQL = "UPDATE DATOSALUMNOS SET APE_NOM =@pApenom, FCH_NAC =@pFchnac, NRODOC =@pNrodoc, COD_ECIV = @pCodEciv, ";
            SQL = SQL + "TEL_CELU =@pTelcelu, E_MAIL =@pEmail, FOTO_JPG =@pFotojpg, LEGAJO_PDF =@pLegajopdf, OBSERVAC =@pObservac ";
            SQL = SQL + "WHERE LEGAJO='" + pLegajo + "'";

            SqlConnection con = new SqlConnection(Conexion.sConnection);
            SqlCommand com = new SqlCommand(SQL, con);
            SqlParameter retParam = com.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);

            retParam.Direction = ParameterDirection.ReturnValue;
            com.Parameters.Add("@pLegajo", SqlDbType.Int, 1).Value = pLegajo;
            com.Parameters.Add("@pApenom", SqlDbType.VarChar, 100).Value = pApenom;
            com.Parameters.Add("@pFchnac", SqlDbType.DateTime).Value = pFchnac;
            com.Parameters.Add("@pNrodoc", SqlDbType.Int).Value = pNrodoc;
            com.Parameters.Add("@pCodEciv", SqlDbType.Int).Value = pCodEciv;
            com.Parameters.Add("@pTelcelu", SqlDbType.VarChar, 15).Value = pTelcelu;
            com.Parameters.Add("@pEmail", SqlDbType.VarChar, 30).Value = pEmail;
            com.Parameters.Add("@pFotojpg", SqlDbType.VarChar, 16).Value = pFotojpg;
            com.Parameters.Add("@pLegajopdf", SqlDbType.VarChar, 16).Value = pLegajopdf;
            com.Parameters.Add("@pObservac", SqlDbType.VarChar, 250).Value = pObservac;

            try
            {
                con.Open();
                objTransaction = con.BeginTransaction();
                com.Transaction = objTransaction;

                com.ExecuteNonQuery();
                Identidad = Convert.ToInt32(retParam.Value);
                //commit: confirma los cambios que se han ejecutado en el contexto de la transacion
                objTransaction.Commit(); // no hubo error si se confirman los cambios

            }
            catch (SqlException ex)
            {
                //Rollback: cancela los cambios que se han ejecutado en el contexto de la transaccion
                objTransaction.Rollback();
                throw new Exception("Error en Base de Datos" + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Dispose();
                com.Dispose();
            }
            return Identidad;
        }

        public DataSet EliminarAlumno(int plegajo)
        {
            SQL = "DELETE FROM DATOSALUMNOS ";
            SQL = SQL + "WHERE LEGAJO='" + plegajo + "'";

            SqlConnection con = new SqlConnection(Conexion.sConnection);
            DataSet objDataset = new DataSet();
            SqlCommand com = new SqlCommand(SQL, con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = com;

            try
            {
                da.Fill(objDataset);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error en la base de datos " + ex.Message);
            }
            finally
            {
                con.Dispose();
                com.Dispose();
            }
            return objDataset;
        }
    }
}
