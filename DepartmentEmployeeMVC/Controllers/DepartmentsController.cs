﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepartmentEmployeeMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DepartmentEmployeeMVC.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Department
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name FROM Departments";
                    var reader = cmd.ExecuteReader();
                    var departments = new List<Department>();
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                           
                        });
                    }
                    reader.Close();
                    return View(departments);
                }
            }
        }

        // GET: Department/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id AS DepartmentId, d.Name AS DepartmentName, e.FirstName, 
                                        e.LastName, e.Id as EmployeeId
                                        FROM Departments d
                                        LEFT JOIN Employees e ON d.Id = e.DepartmentId
                                        WHERE d.Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    var reader = cmd.ExecuteReader();

                    Department department = null;

                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Employees = new List<Employee>()
                            };
                        }
                        var hasEmployee = !reader.IsDBNull(reader.GetOrdinal("EmployeeId"));
                        if (hasEmployee)
                        {
                            department.Employees.Add(new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            });
                        };
                    }

                    reader.Close();

                    if (department == null)
                    {
                        return NotFound();
                    }
                    return View(department);



                }
            }
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"INSERT INTO Departments (Name)
                                            VALUES (@Name)";

                            cmd.Parameters.Add(new SqlParameter("@Name", department.Name));

                            cmd.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    return View();
                }
            }
        }

        // GET: Department/Edit/5
        public ActionResult Edit(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name
                                        FROM Departments 
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };

                        reader.Close();
                        return View(department);
                    }
                    reader.Close();
                    return NotFound();
                }
            }

        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Department department)
        {
            {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"UPDATE Departments
                                            SET Name = @Name
                                            WHERE Id = @id";

                            cmd.Parameters.Add(new SqlParameter("@Name", department.Name));
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            cmd.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
        }

        // GET: Department/Delete/5
        public ActionResult Delete(int id)
        {
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, Name FROM Departments WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        var reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            var department = new Department
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Id = reader.GetInt32(reader.GetOrdinal("Id"))
                            };

                            reader.Close();
                            return View(department);
                        }
                        return NotFound();
                    }
                }
            }
        }

        // POST: Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Departments WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }

            }
            catch
            {
                return View();
            }
        }
    }
}