using BIQueryScheduler.API.Models;
using BIQueryScheduler.API.Services;
using BIQueryScheduler.API.Services.Interfaces;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BIQueryScheduler.UnitTests.Services
{
    public class RunBIStoredProceduresServiceTests
    {
        [Fact]
        public void Run_BI_StoredProcedures()
        {
            //Arrange
            BIDataBase biDataBase = new BIDataBase()
            {
                StoredProcedures = new StoredProcedures()
                {
                    SP1 = "Test_StoredProcedure",
                    SP2 = "Test_StoredProcedure",
                    SP3 = "Test_StoredProcedure",
                    SP4 = "Test_StoredProcedure",
                    SP5 = "Test_StoredProcedure",
                    SP6 = "Test_StoredProcedure",
                    SP7 = "Test_StoredProcedure",
                    SP8 = "Test_StoredProcedure",
                    SP9 = "Test_StoredProcedure"
                },
                ConnectionString = "Test_ConnectionString"
            };

            var runBIStoredProceduresService = new RunBIStoredProceduresService();

            //Assert
            Should.Throw<ArgumentException>(() => runBIStoredProceduresService.RunBIStoredProcedures(biDataBase));
        }
    }
}
