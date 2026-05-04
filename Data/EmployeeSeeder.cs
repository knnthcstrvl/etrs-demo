using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data
{
    public static class EmployeeSeeder
    {
        public static async Task SeedAsync(ProcurementDbContext db)
        {
            if (await db.Employees.AnyAsync())
            {
                return;
            }

            var employees = new List<Employee>
            {
                new()
                {
                    IdNumber = "BSWM-2026-0001",
                    LastName = "Santos",
                    FirstName = "Maria",
                    MiddleName = "Lopez",
                    DivisionCenterUnit = "Soils Survey Division (SSD)",
                    Designation = "Chief Agriculturist",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0002",
                    LastName = "Reyes",
                    FirstName = "John Paul",
                    MiddleName = "Rivera",
                    DivisionCenterUnit = "Agricultural Land Management and Evaluation Division (ALMED)",
                    Designation = "Chief Agriculturist",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0003",
                    LastName = "Dela Cruz",
                    FirstName = "Angela",
                    MiddleName = "Torres",
                    DivisionCenterUnit = "Soil Conservation Management Division (SCMD)",
                    Designation = "Chief Agriculturist",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0004",
                    LastName = "Garcia",
                    FirstName = "Mark Anthony",
                    MiddleName = "Fernandez",
                    DivisionCenterUnit = "Water Resources Management Division (WRMD)",
                    Designation = "Chief Engineer",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0005",
                    LastName = "Mendoza",
                    FirstName = "Clarisse",
                    MiddleName = "Bautista",
                    DivisionCenterUnit = "Laboratory Services Division (LSD)",
                    Designation = "Chief Agriculturist",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0006",
                    LastName = "Villanueva",
                    FirstName = "Karen",
                    MiddleName = "Soriano",
                    DivisionCenterUnit = "Soil and Water Resources Research Division (SWRRD)",
                    Designation = "Chief Science Research Specialist",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0007",
                    LastName = "Navarro",
                    FirstName = "Bert",
                    MiddleName = "Luna",
                    DivisionCenterUnit = "Geomatics and Soil Information Technology Division (GSITD)",
                    Designation = "Information Technology Officer III",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0008",
                    LastName = "Castro",
                    FirstName = "Oscar",
                    MiddleName = "Ramos",
                    DivisionCenterUnit = "National Soil and Water Resources Research and Development Center for Lowland-Upland Pedo-Ecological Zone (LUPEZ)",
                    Designation = "Agricultural Center Chief IV",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0009",
                    LastName = "Espinosa",
                    FirstName = "Joven",
                    MiddleName = "Diaz",
                    DivisionCenterUnit = "National Soil and Water Resources Research and Development Center for Hillyland Pedo-Ecological Zone (HILLPEZ)",
                    Designation = "Agricultural Center Chief IV",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0010",
                    LastName = "Apolinario",
                    FirstName = "Henry",
                    MiddleName = "Morales",
                    DivisionCenterUnit = "National Soil and Water Resources Research and Development Center for Highland Pedo-Ecological Zone (HIGHPEZ)",
                    Designation = "Agricultural Center Chief IV",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0011",
                    LastName = "Aquino",
                    FirstName = "Rose Anne",
                    MiddleName = "Perez",
                    DivisionCenterUnit = "Office of the Director",
                    Designation = "Administrative Officer V",
                    Status = "Active"
                },
                new()
                {
                    IdNumber = "BSWM-2026-0012",
                    LastName = "Lim",
                    FirstName = "Daniel",
                    MiddleName = "Co",
                    DivisionCenterUnit = "Personnel Section",
                    Designation = "Administrative Officer IV",
                    Status = "Active"
                }
            };

            await db.Employees.AddRangeAsync(employees);
            await db.SaveChangesAsync();
        }
    }
}
