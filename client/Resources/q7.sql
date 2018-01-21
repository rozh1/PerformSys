SELECT
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
	L_YEAR;