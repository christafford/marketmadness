--------------------
-- Table: StockNames
--------------------

CREATE TABLE [dbo].[StockNames](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Stock] [varchar](10) NOT NULL,
	[Exchange] [varchar](50) NOT NULL,
	[Company_Name] [varchar](1000) NULL,
	[Country] [varchar](200) NULL,
	[HasData] [bit] NULL,
 CONSTRAINT [PK_StockNames] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

-----------------
-- Table: Sectors
-----------------

CREATE TABLE [dbo].[Sectors](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Sector] [varchar](500) NOT NULL,
 CONSTRAINT [PK_Sectors] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

--------------------
-- Table: Industries
--------------------

CREATE TABLE [dbo].[Industries](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sector_id] [int] NOT NULL,
	[Iindustry] [varchar](500) NOT NULL,
 CONSTRAINT [PK_Industries] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Industries]  WITH CHECK ADD  CONSTRAINT [FK_Industries_Sectors] FOREIGN KEY([sector_id])
REFERENCES [dbo].[Sectors] ([id])
ALTER TABLE [dbo].[Industries] CHECK CONSTRAINT [FK_Industries_Sectors]

--------------
-- Table: Logs
--------------

CREATE TABLE [dbo].[Logs](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[message] [text] NOT NULL,
	[exception_message] [text] NOT NULL,
	[exception_stacktrace] [text] NOT NULL,
	[transaction_date] [datetime] NOT NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

----------------
-- Table: quotes
----------------

CREATE TABLE [dbo].[quotes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[stock_id] [int] NOT NULL,
	[day] [datetime] NOT NULL,
	[open] [decimal](18, 2) NOT NULL,
	[high] [decimal](18, 2) NOT NULL,
	[low] [decimal](18, 2) NOT NULL,
	[close] [decimal](18, 2) NOT NULL,
	[volume] [bigint] NOT NULL,
	[adjusted_close] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_quotes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[quotes]  WITH CHECK ADD  CONSTRAINT [FK_quotes_StockNames] FOREIGN KEY([stock_id])
REFERENCES [dbo].[StockNames] ([id])
ALTER TABLE [dbo].[quotes] CHECK CONSTRAINT [FK_quotes_StockNames]

---------------------
-- Table: predictions
---------------------

CREATE TABLE [dbo].[predictions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[day] [datetime] NOT NULL,
	[stock] [varchar](50) NOT NULL,
	[NextWeekClose] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_predictions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

----------------------
-- Table: CompanyStats
----------------------

CREATE TABLE [dbo].[CompanyStats](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[stock_id] [int] NOT NULL,
	[scrape_day] [datetime] NOT NULL,
	[market_cap] [bigint] NULL,
	[enterprise_value] [bigint] NULL,
	[trailing_pe] [decimal](18, 2) NULL,
	[forward_pe] [decimal](18, 2) NULL,
	[peg_ratio] [decimal](18, 2) NULL,
	[price_sales] [decimal](18, 2) NULL,
	[price_book] [decimal](18, 2) NULL,
	[ent_value_rev] [decimal](18, 2) NULL,
	[ent_value_ebitda] [decimal](18, 2) NULL,
	[fiscal_year_ends] [varchar](100) NULL,
	[most_recent_quarter] [varchar](100) NULL,
	[profit_margin] [decimal](18, 2) NULL,
	[operating_margin] [decimal](18, 2) NULL,
	[return_on_assets] [decimal](18, 2) NULL,
	[return_on_equity] [decimal](18, 2) NULL,
	[revenue] [bigint] NULL,
	[revenue_per_share] [decimal](18, 2) NULL,
	[qrt_rev_growth] [decimal](18, 2) NULL,
	[gross_profit] [bigint] NULL,
	[ebitda] [bigint] NULL,
	[net_income_a_c] [bigint] NULL,
	[diluted_eps] [decimal](18, 2) NULL,
	[qrt_earnings_growth] [decimal](18, 2) NULL,
	[total_cash] [bigint] NULL,
	[cash_per_share] [decimal](18, 2) NULL,
	[total_debt] [bigint] NULL,
	[total_debt_equit] [decimal](18, 2) NULL,
	[current_ratio] [decimal](18, 2) NULL,
	[book_value_p_share] [decimal](18, 2) NULL,
	[operating_cash_flow] [bigint] NULL,
	[levered_free_cash_flow] [bigint] NULL,
 CONSTRAINT [PK_CompanyStats] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[CompanyStats]  WITH CHECK ADD  CONSTRAINT [FK_CompanyStats_StockNames] FOREIGN KEY([stock_id])
REFERENCES [dbo].[StockNames] ([id])
ALTER TABLE [dbo].[CompanyStats] CHECK CONSTRAINT [FK_CompanyStats_StockNames]

------------------------
-- Table: CompanyProfile
------------------------

CREATE TABLE [dbo].[CompanyProfile](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[stock_id] [int] NOT NULL,
	[scrape_day] [datetime] NOT NULL,
	[sector_id] [int] NULL,
	[industry_id] [int] NULL,
	[num_employees] [int] NULL,
	[summary] [text] NULL,
	[cgq] [decimal](18, 2) NULL,
	[avg_executive_age] [int] NULL,
	[avg_executive_pay] [bigint] NULL,
 CONSTRAINT [PK_CompanyProfile] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[CompanyProfile]  WITH CHECK ADD  CONSTRAINT [FK_CompanyProfile_Industries] FOREIGN KEY([industry_id])
REFERENCES [dbo].[Industries] ([id])
ALTER TABLE [dbo].[CompanyProfile] CHECK CONSTRAINT [FK_CompanyProfile_Industries]
ALTER TABLE [dbo].[CompanyProfile]  WITH CHECK ADD  CONSTRAINT [FK_CompanyProfile_Sectors] FOREIGN KEY([sector_id])
REFERENCES [dbo].[Sectors] ([id])
ALTER TABLE [dbo].[CompanyProfile] CHECK CONSTRAINT [FK_CompanyProfile_Sectors]
ALTER TABLE [dbo].[CompanyProfile]  WITH CHECK ADD  CONSTRAINT [FK_CompanyProfile_StockNames] FOREIGN KEY([stock_id])
REFERENCES [dbo].[StockNames] ([id])
ALTER TABLE [dbo].[CompanyProfile] CHECK CONSTRAINT [FK_CompanyProfile_StockNames]

-----------------
-- Table: Weather
-----------------

CREATE TABLE [dbo].[Weather](
	[day] [datetime] NOT NULL,
	[from_avg] [int] NOT NULL,
 CONSTRAINT [PK_Weather] PRIMARY KEY CLUSTERED 
(
	[day] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

-------------------------
-- View: simplestockquote
-------------------------

CREATE view [dbo].[simplestockquote] as
select s.id, stock, q.day, adjusted_close, volume, industry_id, forward_pe, sector_id,
avg_executive_age, num_employees, net_income_a_c, total_debt, enterprise_value, w.from_avg
from stocknames s
inner join quotes q on s.id = q.stock_id
inner join companyprofile cp on s.id = cp.stock_id and
	cp.id = (
		select top 1 cp2.id
		from companyprofile cp2
		where cp2.stock_id = s.id
		order by abs(convert(float, cp2.scrape_day - q.day))
	)
inner join companystats cs on cs.stock_id = s.id and
	cs.id = (
		select top 1 cs2.id
		from companystats cs2
		where cs2.stock_id = s.id
		order by abs(convert(float, cs2.scrape_day - q.day))
	)
inner join weather w on w.day = q.day
where cp.industry_id is not null
and cs.forward_pe is not null
and cp.sector_id is not null
and avg_executive_age is not null
and num_employees is not null
and net_income_a_c is not null
and total_debt is not null
and enterprise_value is not null
and volume > 0