using System;
using System.Data;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataTableFormatters.Tests
{
    [TestClass]
    public class DataTableExtensionsTests
    {
        [TestMethod]
        public void GetStringRepresentation_WhenAllNormal_Success()
        {
            var table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("very long column name", typeof(string));

            table.Rows.Add(1, "f", "a");
            table.Rows.Add(2, "the longest", "b");
            table.Rows.Add(3, "g", "c");

            var rep = table.GetStringRepresentation();

            rep.Should().Be(
                "+----+-------------+-----------------------+" + Environment.NewLine +
                "| id | name        | very long column name |" + Environment.NewLine +
                "+----+-------------+-----------------------+" + Environment.NewLine +
                "| 1  | f           | a                     |" + Environment.NewLine +
                "| 2  | the longest | b                     |" + Environment.NewLine +
                "| 3  | g           | c                     |" + Environment.NewLine +
                "+----+-------------+-----------------------+" + Environment.NewLine);
        }

        [TestMethod]
        public void GetStringRepresentation_WhenTableWithoutRows_Success()
        {
            var table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("very long column name", typeof(string));

            var rep = table.GetStringRepresentation();

            rep.Should().Be(
                "+----+------+-----------------------+" + Environment.NewLine +
                "| id | name | very long column name |" + Environment.NewLine +
                "+----+------+-----------------------+" + Environment.NewLine +
                "+----+------+-----------------------+" + Environment.NewLine);
        }

        [TestMethod]
        public void GetStringRepresentation_WhenTableWithoutColumns_Success()
        {
            var table = new DataTable();

            var rep = table.GetStringRepresentation();

            rep.Should().Be(
                "+--+" + Environment.NewLine +
                "|  |" + Environment.NewLine +
                "+--+" + Environment.NewLine +
                "+--+" + Environment.NewLine);
        }

        [TestMethod]
        public void GetStringRepresentation_WhenDbNull_PrintsNull()
        {
            var table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("very long column name", typeof(string));

            table.Rows.Add(1, "f", "a");
            table.Rows.Add(2, "the longest", DBNull.Value);
            table.Rows.Add(3, "g", "c");

            var rep = table.GetStringRepresentation();

            rep.Should().Be(
                "+----+-------------+-----------------------+" + Environment.NewLine +
                "| id | name        | very long column name |" + Environment.NewLine +
                "+----+-------------+-----------------------+" + Environment.NewLine +
                "| 1  | f           | a                     |" + Environment.NewLine +
                "| 2  | the longest | NULL                  |" + Environment.NewLine +
                "| 3  | g           | c                     |" + Environment.NewLine +
                "+----+-------------+-----------------------+" + Environment.NewLine);
        }

        [TestMethod]
        public void GetStringRepresentation_WhenRightAlign_Success()
        {
            var table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("very long column name", typeof(string));

            table.Rows.Add(1, "f", "a");
            table.Rows.Add(2, "the longest", DBNull.Value);
            table.Rows.Add(3, "g", "c");

            var rep = table.GetStringRepresentation(
                new RightAlign("id"),
                new RightAlign("name"));

            rep.Should().Be(
                "+----+-------------+-----------------------+" + Environment.NewLine +
                "| id |        name | very long column name |" + Environment.NewLine +
                "+----+-------------+-----------------------+" + Environment.NewLine +
                "|  1 |           f | a                     |" + Environment.NewLine +
                "|  2 | the longest | NULL                  |" + Environment.NewLine +
                "|  3 |           g | c                     |" + Environment.NewLine +
                "+----+-------------+-----------------------+" + Environment.NewLine);
        }

        [TestMethod]
        public void GetStringRepresentation_WhenCustomFormat_Success()
        {
            var table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("very long column name", typeof(double));

            table.Rows.Add(1, "f", 1.0);
            table.Rows.Add(2, "the longest", 2.3456);
            table.Rows.Add(3, "g", -98765);

            var rep = table.GetStringRepresentation(
                new RightAlign("id"),
                new CustomFormat("very long column name", "#.00"));

            rep.Should().Be(
                "+----+-------------+-----------------------+" + Environment.NewLine +
                "| id | name        | very long column name |" + Environment.NewLine +
                "+----+-------------+-----------------------+" + Environment.NewLine +
                "|  1 | f           | 1.00                  |" + Environment.NewLine +
                "|  2 | the longest | 2.35                  |" + Environment.NewLine +
                "|  3 | g           | -98765.00             |" + Environment.NewLine +
                "+----+-------------+-----------------------+" + Environment.NewLine);
        }
    }
}