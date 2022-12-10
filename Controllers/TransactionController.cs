﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elaborate.Elaborate.Entities;
using Elaborate.Entities;
using AutoMapper;
using System.Text.Json;
using MySql.Data.MySqlClient;
using Elaborate.Models;

namespace Elaborate.Controllers
{

    //[Route("api/transaction")]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public TransactionController(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        //[HttpGet]
        /// <summary>
        /// Pobranie listy transakcji
        /// </summary>
        /// <returns>Listę transakcji</returns>
        [HttpGet("transactions")]
        //[Route("transactions")]
         public ActionResult<IEnumerable<Transaction>> GetAll()
         {
             var transactions = _dbContext
                 .Transactions
                 .ToList();

             return Ok(transactions);
         }
        /// <summary>
        /// Pobranie transakcji po Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>transakcje</returns>
         [HttpGet("{id}")]
         public ActionResult<Transaction> Get([FromRoute] int id)
         {
             var transaction = _dbContext
                 .Transactions
                 .FirstOrDefault(r => r.Id == id);

             if (transaction is null)
             {
                 return NotFound();
             }

             return Ok(transaction);
         }

        [HttpGet("filterById")]
        public ActionResult<Transaction> FilterByCateId([FromRoute] int id)
        {
            var transactions = _dbContext
                .Transactions
                .Where(r => r.TransCategoryId == id)
                .ToList();

            return Ok(transactions);
        }

        public int GetNewId()
        {
            int newId = 0;
            try
            {
                string mySqlConnectionString = "server=146.59.126.32;port=3306;uid=user;pwd=Yg5udzLxxw9ADsT;database=elaborate-db";

                using var con = new MySqlConnection(mySqlConnectionString);
                con.Open();
                string query = "SELECT MAX(Id) FROM Transactions ";
                var cmd = new MySqlCommand(query, con);
                var maxId = Int32.Parse(cmd.ExecuteScalar().ToString());
                newId = maxId + 1;
                con.Close();
                
            }
            catch (Exception ex)
            {
                
            }
            return newId;
        }


        /// <summary>
        /// Tworzenie transakcji
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// 
        //[Route("addTransactions")]
        [HttpPost("addTransaction")]
        public ActionResult CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            
            var transaction = _mapper.Map<Transaction>(dto);
            //transaction.Id = GetNewId();
            //transaction.Date = d;
            transaction.AccountId = 7;
            transaction.TransCategoryId = 1;
            transaction.Title = "New title";
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            var transactions = _dbContext
                 .Transactions
                 .ToList();

            decimal transactionSum = transactions.Sum(t => t.Value);

            Object[] resultArr = new Object[] { transactions, transactionSum };
            //var resultArr[] = { transactions, transactionSum } ;
            //resultArr[0] = transactions;
            //resultArr[1] = transactionSum;

            


            return Ok(resultArr);

            //return Created($"/api/transaction/{transaction.Id}", null);
        }

        /// <summary>
        /// Zmiana danych transakcji // Bartosz Truszkowski
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTransaction(int id, [FromBody] UpdateTransaction updateTransaction)
        {
            var transactionToUpdate = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);

            if (transactionToUpdate is null)
                return NotFound("Nie znaleziono transakcji o podanym id");

            transactionToUpdate.Comment = updateTransaction.Comment;
            transactionToUpdate.Value = updateTransaction.Value;
            transactionToUpdate.Title = updateTransaction.Title;

            await _dbContext.SaveChangesAsync();

            return Ok(transactionToUpdate);
        }


        //[HttpGet]
        //public ActionResult<IEnumerable<Transaction>> GetAll()
        //{
        //    var transactions = _dbContext
        //        .Transactions
        //        .ToList();

        //    var transactionsDtos = _mapper.Map<List<>  //Zaczęto robić mapowanie

        //    return Ok(transactions);
        //}

        //[HttpGet("{id}")]
        //public ActionResult<Transaction> Get([FromRoute] int id)
        //{
        //    var transaction = _dbContext
        //        .Transactions
        //        .FirstOrDefault(r => r.Id == id);

        //    if (transaction is null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(transaction);
        //}

        [HttpGet("transactionsSum")]
        //[Route("transactionsSum")]
        public ActionResult<Transaction> GetSumOfTransactions()
        {
            decimal transactionSum = _dbContext
                .Transactions
                .Sum(t => t.Value);
            //string jsonString = JsonSerializer.Serialize(transactionSum);
            // nie sprawdzam czy istnieje jakikolwiek rekord, najwyżej zwróci 0
            return Ok(transactionSum);
        }

        //[HttpPost]
        //public ActionResult<IEnumerable<Transaction>> Add(Transaction trans)
        //{
        //    if (_dbContext.Database.CanConnect())
        //    {
        //        _dbContext.Transactions.Add(trans);
        //        _dbContext.SaveChanges();
        //    }
        //    return Ok(trans);
        //}

        [HttpGet("filterByMonth")]
        public ActionResult<Transaction> FilterByMonth([FromRoute] DateTime date)
        {
            var transactions = _dbContext
                .Transactions
                .Where(r => r.Date.Month == date.Month)
                .ToList();

            return Ok(transactions);
        }

    }

}
