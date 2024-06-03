﻿using EmpresaTajAzure.Data;
using EmpresaTajAzure.Models;
using EmpresaTajAzure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmpresaTajAzure.Controllers
{
    public class UsuariosController : Controller
    {

        private ServiceApiTajamar service;
        private ApplicationContext context;

        public UsuariosController(ServiceApiTajamar service, ApplicationContext context)
        {
            this.service = service;
            this.context = context;
        }

        //public async Task<IActionResult> _AlumnosSeleccionados(int idEmpresa)
        //{
        //    List<UsuarioEmpresa> usuarios = await this.service.FindUsuariosPorEmpresa(idEmpresa);
        //    return PartialView("_AlumnosSeleccionados", usuarios);
        //}

        public async Task<IActionResult> _AlumnosSeleccionados(int idEmpresa)
        {
            List<UsuarioEmpresa> usuarios = await this.service.FindUsuariosPorEmpresa(idEmpresa);
            var tuple = new Tuple<int, List<UsuarioEmpresa>>(idEmpresa, usuarios);
            return PartialView("_AlumnosSeleccionados", tuple);
        }




        public async Task<IActionResult> ListaUsuarios()
        {
            string token = HttpContext.Session.GetString("TOKEN");
            List<UsuarioEmpresa> usuarios = await
                this.service.GetUsuariosAsync(token);
            return View(usuarios);
        }

        public async Task<IActionResult> AlumnosList()
        {
            string token = HttpContext.Session.GetString("TOKEN");
            List<UsuarioEmpresa> usuarios = await
                this.service.GetUsuariosAsync(token);
            return View(usuarios);
        }

        public async Task<IActionResult> _Details(int id)
        {
            UsuarioEmpresaConNombre usuario = await this.service.FindUsuarioNombreEmpresasAsync(id);
            // return View(usuario);
            return PartialView("_Details", usuario);
        }


        public async Task<IActionResult> PerfilUsuario()
        {
            int idUsuario = int.Parse(HttpContext.Session.GetString("IDUSUARIO"));

            UsuarioEmpresaConNombre usuario = await this.service.FindUsuarioNombreEmpresasAsync(idUsuario);
          
                      
            return View(usuario);
        }

        public async Task<IActionResult> PerfilUsuarioAdmin()
        {
            int idUsuario = int.Parse(HttpContext.Session.GetString("IDUSUARIO"));

            UsuarioEmpresa usuario = await this.service.FindUsuarioAsync(idUsuario);


            return View(usuario);
        }



        [Authorize]
        public IActionResult Perfil()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            // Eliminar todas las sesiones
            HttpContext.Session.Clear();

            // Sign out de todos los esquemas de autenticación
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, "Identity.Application");
        }

        [Authorize]
        public async Task<IActionResult> Registro()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userEmail = User.Identity.Name;
                UsuarioEmpresa usuario = await this.service.FindUsuarioEmailAsync(userEmail);

                if (usuario != null)
                {
                    string token = await this.service.GetTokenAsync(userEmail, usuario.IdUsuario);
                    if (token == null)
                    {
                        ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                    }
                    else
                    {
                        ViewData["MENSAJE"] = "Ya tienes tu token";
                        HttpContext.Session.SetString("TOKEN", token);
                        HttpContext.Session.SetString("NOMBREUSUARIO", usuario.Nombre);
                        HttpContext.Session.SetString("IDUSUARIO", usuario.IdUsuario.ToString());
                        HttpContext.Session.SetString("IDCLASE", usuario.IdClase.ToString());
                        HttpContext.Session.SetString("ROLE", usuario.Role);
                        HttpContext.Session.SetString("LINKEDIN", usuario.Linkedin);
                        HttpContext.Session.SetString("EMAIL", usuario.Email);
                        HttpContext.Session.SetString("EMPRESA1", usuario.Emp_1Id.ToString());
                        HttpContext.Session.SetString("EMPRESA2", usuario.Emp_2Id.ToString());
                        HttpContext.Session.SetString("EMPRESA3", usuario.Emp_3Id.ToString());
                        HttpContext.Session.SetString("EMPRESA4", usuario.Emp_4Id.ToString());
                        HttpContext.Session.SetString("EMPRESA5", usuario.Emp_5Id.ToString());
                        HttpContext.Session.SetString("EMPRESA6", usuario.Emp_6Id.ToString());
                        return RedirectToAction("Perfil", "Usuarios");

                    }
                }

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(string email, int idClase, string nombre, string linkedin)
        {
            UsuarioEmpresa usuario = new UsuarioEmpresa();
            usuario.IdUsuario = 0;
            usuario.IdClase = idClase;
            usuario.Nombre = nombre;
            usuario.Role = "Admin";
            usuario.Linkedin = linkedin;
            usuario.Email = email;
            usuario.Emp_1Id = null;
            usuario.Emp_2Id = null;
            usuario.Emp_3Id = null;
            usuario.Emp_4Id = null;
            usuario.Emp_5Id = null;
            usuario.Emp_6Id = null;

            await this.service.InsertusuarioAsync(usuario.IdUsuario, usuario.IdClase, usuario.Nombre, usuario.Role, usuario.Linkedin, usuario.Email, usuario.Emp_1Id, usuario.Emp_2Id, usuario.Emp_3Id, usuario.Emp_4Id, usuario.Emp_5Id, usuario.Emp_6Id);

            

            return RedirectToAction("Registro", "Usuarios");
        }
        public async Task<IActionResult> InsertarEntrevistaAlumno(int idEmpresa, DateTime fechaEntrevista, string estado)
        {
            int idUsuario = int.Parse(HttpContext.Session.GetString("IDUSUARIO"));

            EntrevistaAlumno entrevista = new EntrevistaAlumno();
            entrevista.IdentEntrevista = 0;
            entrevista.IdAlumno = idUsuario;
            entrevista.IdEmpresa = idEmpresa;
            entrevista.FechaEntrevista = fechaEntrevista;
            entrevista.Estado = estado;

            await this.service.InsertEntrevistaAsync(entrevista.IdentEntrevista, entrevista.IdAlumno, entrevista.IdEmpresa, entrevista.FechaEntrevista, entrevista.Estado);
            return RedirectToAction("ListaEntrevistas");
        }

        public async Task<IActionResult> InsertarEmpresaAlumno(int? idempresa1, int? idempresa2, int? idempresa3, int? idempresa4, int? idempresa5, int? idempresa6)
        {
           
            int idUsuario = int.Parse(HttpContext.Session.GetString("IDUSUARIO"));
            int idClase = int.Parse(HttpContext.Session.GetString("IDCLASE"));
            string nombre = HttpContext.Session.GetString("NOMBREUSUARIO");
            string role = HttpContext.Session.GetString("ROLE");
            string linkedin = HttpContext.Session.GetString("LINKEDIN");
            string email = HttpContext.Session.GetString("EMAIL");
            await this.service.UpdateUsuarioAsync(idUsuario, idClase, nombre, role, linkedin, email, idempresa1, idempresa2, idempresa3, idempresa4, idempresa5, idempresa6);

            // Aquí puedes devolver una respuesta JSON u otro tipo de respuesta según tu necesidad
            return View();
        }



    }
}
