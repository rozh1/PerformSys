#region Copyright
/*
 * Copyright 2013-2018 Roman Klassen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */
#endregion

﻿using System;
using System.Collections.Generic;
using Balancer.Common.Logger.Enums;
using client.Config.Data;

namespace client.Config
{
    internal class ConfigInit
    {
        public ConfigInit()
        {
            Config = new ClientConfig
            {
                Server = new Server
                {
                    Host = "localhost",
                    Port = 3409,
                },
                Log = new Log
                {
                    LogFile = "clientLog.txt",
                    StatsFile = "clientStats.csv",
                    LogDir = "",
                    WriteToConsole = true,
                    LogLevel = LogLevel.DEBUG,
                    LogMode = LogMode.MULTIPLE
                },
                QuerySequence = new Data.QuerySequence
                {
                    Mode = QuerySequenceMode.FromList,
                    List = new[]
                    {
                        1
                    }
                },
                Scenario = new Scenario
                {
                    ClientCount = 50,
                    StartTime = DateTime.Now,
                    ScenarioSteps = new[]
                    {
                        new ScenarioStep
                        {
                            Action = ScenarioActions.Work,
                            Duration = new TimeSpan(0, 0, 0, 1)
                        }
                    }
                }
            };

            Queries = new QueriesList()
            {
                Queries = new List<QueryConfig>()
                {
                    new QueryConfig() { Number = 1, Sql = @"SELECT
	L_RETURNFLAG,
	L_LINESTATUS,
	SUM(L_QUANTITY) AS SUM_QTY,
	SUM(L_EXTENDEDPRICE) AS SUM_BASE_PRICE,
	SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT)) AS SUM_DISC_PRICE,
	SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT) * (1 + L_TAX)) AS SUM_CHARGE,
	AVG(L_QUANTITY) AS AVG_QTY,
	AVG(L_EXTENDEDPRICE) AS AVG_PRICE,
	AVG(L_DISCOUNT) AS AVG_DISC,
	COUNT(*) AS COUNT_ORDER
FROM
	LINEITEM
WHERE
	L_SHIPDATE <= DATE '1998-12-01' - INTERVAL '90' DAY
GROUP BY
	L_RETURNFLAG,
	L_LINESTATUS
ORDER BY
	L_RETURNFLAG,
	L_LINESTATUS;"},
                    new QueryConfig() { Number = 2, Sql = @"SELECT
	S_ACCTBAL,
	S_NAME,
	N_NAME,
	P_PARTKEY,
	P_MFGR,
	S_ADDRESS,
	S_PHONE,
	S_COMMENT
FROM
	PART,
	SUPPLIER,
	PARTSUPP,
	NATION,
	REGION
WHERE
	P_PARTKEY = PS_PARTKEY
	AND S_SUPPKEY = PS_SUPPKEY
	AND P_SIZE = 48
	AND P_TYPE LIKE '%NICKEL'
	AND S_NATIONKEY = N_NATIONKEY
	AND N_REGIONKEY = R_REGIONKEY
	AND R_NAME = 'AMERICA'
	AND PS_SUPPLYCOST = (
		SELECT
			MIN(PS_SUPPLYCOST)
		FROM
			PARTSUPP,
			SUPPLIER,
			NATION,
			REGION
		WHERE
			P_PARTKEY = PS_PARTKEY
			AND S_SUPPKEY = PS_SUPPKEY
			AND S_NATIONKEY = N_NATIONKEY
			AND N_REGIONKEY = R_REGIONKEY
			AND R_NAME = 'AMERICA'
	)
ORDER BY
	S_ACCTBAL DESC,
	N_NAME,
	S_NAME,
	P_PARTKEY;"},
                    new QueryConfig() { Number = 3, Sql = @"SELECT
	L_ORDERKEY,
	SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT)) AS REVENUE,
	O_ORDERDATE,
	O_SHIPPRIORITY
FROM
	CUSTOMER,
	ORDERS,
	LINEITEM
WHERE
	C_MKTSEGMENT = 'HOUSEHOLD'
	AND C_CUSTKEY = O_CUSTKEY
	AND L_ORDERKEY = O_ORDERKEY
	AND O_ORDERDATE < DATE '1995-03-31'
	AND L_SHIPDATE > DATE '1995-03-31'
GROUP BY
	L_ORDERKEY,
	O_ORDERDATE,
	O_SHIPPRIORITY
ORDER BY
	REVENUE DESC,
	O_ORDERDATE;"},
                    new QueryConfig() { Number = 4, Sql = @"SELECT
    O_ORDERPRIORITY,
    COUNT(*) AS ORDER_COUNT
FROM
    ORDERS
WHERE
    O_ORDERDATE >= DATE '1996-02-01'
    AND O_ORDERDATE < DATE '1996-02-01' + INTERVAL '3' MONTH
    AND EXISTS (
        SELECT
            *
        FROM
            LINEITEM
        WHERE
            L_ORDERKEY = O_ORDERKEY
            AND L_COMMITDATE < L_RECEIPTDATE
    )
GROUP BY
    O_ORDERPRIORITY
ORDER BY
    O_ORDERPRIORITY;"},
                    new QueryConfig() { Number = 5, Sql = @"SELECT
	N_NAME,
	SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT)) AS REVENUE
FROM
	CUSTOMER,
	ORDERS,
	LINEITEM,
	SUPPLIER,
	NATION,
	REGION
WHERE
	C_CUSTKEY = O_CUSTKEY
	AND L_ORDERKEY = O_ORDERKEY
	AND L_SUPPKEY = S_SUPPKEY
	AND C_NATIONKEY = S_NATIONKEY
	AND S_NATIONKEY = N_NATIONKEY
	AND N_REGIONKEY = R_REGIONKEY
	AND R_NAME = 'MIDDLE EAST'
	AND O_ORDERDATE >= DATE '1995-01-01'
	AND O_ORDERDATE < DATE '1995-01-01' + INTERVAL '1' YEAR
GROUP BY
	N_NAME
ORDER BY
	REVENUE DESC;"},
                    new QueryConfig() { Number = 6, Sql = @"SELECT
	SUM(L_EXTENDEDPRICE * L_DISCOUNT) AS REVENUE
FROM
	LINEITEM
WHERE
	L_SHIPDATE >= DATE '1997-01-01'
	AND L_SHIPDATE < DATE '1997-01-01' + INTERVAL '1' YEAR
	AND L_DISCOUNT BETWEEN 0.07 - 0.01 AND 0.07 + 0.01
	AND L_QUANTITY < 24;"},
                    new QueryConfig() { Number = 7, Sql = @"SELECT
	SUPP_NATION,
	CUST_NATION,
	L_YEAR,
	SUM(VOLUME) AS REVENUE
FROM
	(
		SELECT
			N1.N_NAME AS SUPP_NATION,
			N2.N_NAME AS CUST_NATION,
			EXTRACT(YEAR FROM L_SHIPDATE) AS L_YEAR,
			L_EXTENDEDPRICE * (1 - L_DISCOUNT) AS VOLUME
		FROM
			SUPPLIER,
			LINEITEM,
			ORDERS,
			CUSTOMER,
			NATION N1,
			NATION N2
		WHERE
			S_SUPPKEY = L_SUPPKEY
			AND O_ORDERKEY = L_ORDERKEY
			AND C_CUSTKEY = O_CUSTKEY
			AND S_NATIONKEY = N1.N_NATIONKEY
			AND C_NATIONKEY = N2.N_NATIONKEY
			AND (
				(N1.N_NAME = 'IRAQ' AND N2.N_NAME = 'ALGERIA')
				OR (N1.N_NAME = 'ALGERIA' AND N2.N_NAME = 'IRAQ')
			)
			AND L_SHIPDATE BETWEEN DATE '1995-01-01' AND DATE '1996-12-31'
	) AS SHIPPING
GROUP BY
	SUPP_NATION,
	CUST_NATION,
	L_YEAR
ORDER BY
	SUPP_NATION,
	CUST_NATION,
	L_YEAR;"},
                    new QueryConfig() { Number = 8, Sql = @"SELECT
	O_YEAR,
	SUM(CASE
		WHEN NATION = 'IRAN' THEN VOLUME
		ELSE 0
	END) / SUM(VOLUME) AS MKT_SHARE
FROM
	(
		SELECT
			EXTRACT(YEAR FROM O_ORDERDATE) AS O_YEAR,
			L_EXTENDEDPRICE * (1 - L_DISCOUNT) AS VOLUME,
			N2.N_NAME AS NATION
		FROM
			PART,
			SUPPLIER,
			LINEITEM,
			ORDERS,
			CUSTOMER,
			NATION N1,
			NATION N2,
			REGION
		WHERE
			P_PARTKEY = L_PARTKEY
			AND S_SUPPKEY = L_SUPPKEY
			AND L_ORDERKEY = O_ORDERKEY
			AND O_CUSTKEY = C_CUSTKEY
			AND C_NATIONKEY = N1.N_NATIONKEY
			AND N1.N_REGIONKEY = R_REGIONKEY
			AND R_NAME = 'MIDDLE EAST'
			AND S_NATIONKEY = N2.N_NATIONKEY
			AND O_ORDERDATE BETWEEN DATE '1995-01-01' AND DATE '1996-12-31'
			AND P_TYPE = 'STANDARD BRUSHED BRASS'
	) AS ALL_NATIONS
GROUP BY
	O_YEAR
ORDER BY
	O_YEAR;
"},
                    new QueryConfig() { Number = 9, Sql = @"SELECT
	NATION,
	O_YEAR,
	SUM(AMOUNT) AS SUM_PROFIT
FROM
	(
		SELECT
			N_NAME AS NATION,
			EXTRACT(YEAR FROM O_ORDERDATE) AS O_YEAR,
			L_EXTENDEDPRICE * (1 - L_DISCOUNT) - PS_SUPPLYCOST * L_QUANTITY AS AMOUNT
		FROM
			PART,
			SUPPLIER,
			LINEITEM,
			PARTSUPP,
			ORDERS,
			NATION
		WHERE
			S_SUPPKEY = L_SUPPKEY
			AND PS_SUPPKEY = L_SUPPKEY
			AND PS_PARTKEY = L_PARTKEY
			AND P_PARTKEY = L_PARTKEY
			AND O_ORDERKEY = L_ORDERKEY
			AND S_NATIONKEY = N_NATIONKEY
			AND P_NAME LIKE '%SNOW%'
	) AS PROFIT
GROUP BY
	NATION,
	O_YEAR
ORDER BY
	NATION,
	O_YEAR DESC;
"},
                    new QueryConfig() { Number = 10, Sql = @"SELECT
	C_CUSTKEY,
	C_NAME,
	SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT)) AS REVENUE,
	C_ACCTBAL,
	N_NAME,
	C_ADDRESS,
	C_PHONE,
	C_COMMENT
FROM
	CUSTOMER,
	ORDERS,
	LINEITEM,
	NATION
WHERE
	C_CUSTKEY = O_CUSTKEY
	AND L_ORDERKEY = O_ORDERKEY
	AND O_ORDERDATE >= DATE '1994-04-01'
	AND O_ORDERDATE < DATE '1994-04-01' + INTERVAL '3' MONTH
	AND L_RETURNFLAG = 'R'
	AND C_NATIONKEY = N_NATIONKEY
GROUP BY
	C_CUSTKEY,
	C_NAME,
	C_ACCTBAL,
	C_PHONE,
	N_NAME,
	C_ADDRESS,
	C_COMMENT
ORDER BY
	REVENUE DESC;"},
                    new QueryConfig() { Number = 11, Sql = @"SELECT
	PS_PARTKEY,
	SUM(PS_SUPPLYCOST * PS_AVAILQTY) AS VALUE
FROM
	PARTSUPP,
	SUPPLIER,
	NATION
WHERE
	PS_SUPPKEY = S_SUPPKEY
	AND S_NATIONKEY = N_NATIONKEY
	AND N_NAME = 'ALGERIA'
GROUP BY
	PS_PARTKEY HAVING
		SUM(PS_SUPPLYCOST * PS_AVAILQTY) > (
			SELECT
				SUM(PS_SUPPLYCOST * PS_AVAILQTY) * 0.0001000000
			FROM
				PARTSUPP,
				SUPPLIER,
				NATION
			WHERE
				PS_SUPPKEY = S_SUPPKEY
				AND S_NATIONKEY = N_NATIONKEY
				AND N_NAME = 'ALGERIA'
		)
ORDER BY
	VALUE DESC;
"},
                    new QueryConfig() { Number = 12, Sql = @"SELECT
	L_SHIPMODE,
	SUM(CASE
		WHEN O_ORDERPRIORITY = '1-URGENT'
			OR O_ORDERPRIORITY = '2-HIGH'
			THEN 1
		ELSE 0
	END) AS HIGH_LINE_COUNT,
	SUM(CASE
		WHEN O_ORDERPRIORITY <> '1-URGENT'
			AND O_ORDERPRIORITY <> '2-HIGH'
			THEN 1
		ELSE 0
	END) AS LOW_LINE_COUNT
FROM
	ORDERS,
	LINEITEM
WHERE
	O_ORDERKEY = L_ORDERKEY
	AND L_SHIPMODE IN ('AIR', 'SHIP')
	AND L_COMMITDATE < L_RECEIPTDATE
	AND L_SHIPDATE < L_COMMITDATE
	AND L_RECEIPTDATE >= DATE '1994-01-01'
	AND L_RECEIPTDATE < DATE '1994-01-01' + INTERVAL '1' YEAR
GROUP BY
	L_SHIPMODE
ORDER BY
	L_SHIPMODE;
"},
                    new QueryConfig() { Number = 13, Sql = @"select
	c_count,
	count(*) as custdist
from
	(
		select
			c_custkey,
			count(o_orderkey) as c_count
		from
			customer left outer join orders on
				c_custkey = o_custkey
				and o_comment not like '%special%requests%'
		group by
			c_custkey
	) as c_orders
group by
	c_count
order by
	custdist desc,
	c_count desc;"},
                    new QueryConfig() { Number = 14, Sql = @"SELECT
	100.00 * SUM(CASE
		WHEN P_TYPE LIKE 'PROMO%'
			THEN L_EXTENDEDPRICE * (1 - L_DISCOUNT)
		ELSE 0
	END) / SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT)) AS PROMO_REVENUE
FROM
	LINEITEM,
	PART
WHERE
	L_PARTKEY = P_PARTKEY
	AND L_SHIPDATE >= DATE '1995-01-01'
	AND L_SHIPDATE < DATE '1995-01-01' + INTERVAL '1' MONTH;
"}
                }
            };
        }

        public ClientConfig Config { get; private set; }

        public QueriesList Queries { get; private set; }
    }
}