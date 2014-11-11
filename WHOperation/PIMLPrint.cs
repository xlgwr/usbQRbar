using System;
using System.Collections.Generic;
using System.Text;
using System.IO; 
namespace WHOperation
{   
    public class PIMLPrint
    {
        public StringBuilder genPIML() {
            StringBuilder cRet = new StringBuilder();
            return cRet;
        }
        public StringBuilder genPIML(
            String t1_year, String t1_type,String t1_lot,String t1_part,String t1_site,String t1_qty_per2,String t1_qty_tot,String t1_ref,String t1_loc,
            String t1_expi_date,String t1_expi_type,String t1_mfgr_part,String t1_cust_part,String t1_pims_nbr,String t1_date_code,
            String t1_printer,String t1_by,String t1_wt_ind,String t1_wt,String t1_MSD,String t1_user,String t1_msd,String t1_sony,int cNoLabel,String t1_RIR,String t1_nw_po
            ) {
            //cRet = cRet + Convert.ToChar(CharCode));
            //cRet = cRet + (char)CharCode);
            StringBuilder cRet = new StringBuilder();
            //String cRet;
            if (t1_lot.Length == 0)
                t1_lot  = " ";
            if (t1_printer == "1") {
               cRet.Append(Convert.ToChar(123).ToString() +  "D0410,0762,0380|" + Convert.ToChar(125).ToString()+Convert.ToChar(13)); // format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "C|" + Convert.ToChar(125) + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "U2;0030|" + Convert.ToChar(125) + Convert.ToChar(13)); // format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "AX;+000,+000,+00|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); // format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "AY;+10,0|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC019;0077,0017,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC19;" + "_____________________________________________|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC017;0100,0040,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); // format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC17;" + t1_user + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC016;0300,0040,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC16;" + t1_by + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); // format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "XB00;0080,0042,A,3,02,0,0036,+0000000000,000,0,00|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               //cRet.Append(Convert.ToChar(123).ToString() + "RB00;>7" + t1_lot + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RB00;>7" + t1_RIR  + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC001;0077,0106,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)"                     skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC01;" + t1_type + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               //cRet.Append(Convert.ToChar(123).ToString() + "PC002;0260,0104,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)"                     skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC002;0300,0106,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)"                     skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC02;" + t1_loc + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC003;0500,0074,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC03;" + t1_RIR +"  " +  "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC003;0720,0074,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC03;" + t1_nw_po + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC004;0075,0355,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)"                     skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC04;" + t1_part + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC006;0077,0140,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)"                      skip.
               cRet.Append(Convert.ToChar(123).ToString()+ "RC06;" + t1_site + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "XB02;0380,0110,A,3,01,0,0035,+0000000000,000,1,00|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  
               cRet.Append(Convert.ToChar(123).ToString() + "RB02;>7" + t1_qty_per2 + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));                      //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC007;0077,0215,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)"                     skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC07;" + t1_qty_tot + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC008;0077,0175,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)"                      skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC08;Expiry |" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC009;0174,0175,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)"                      skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC09;" + t1_expi_date + " " + '(' + t1_expi_type + ')' + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.

               if (t1_pims_nbr != "") {
                   cRet.Append(Convert.ToChar(123).ToString() + "XB03;0270,0180,A,3,02,0,0045,+0000000000,000,1,00|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
                   cRet.Append(Convert.ToChar(123).ToString() + "RB03;>7" + t1_pims_nbr + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //format "x(60)" skip.
               } else {
                   cRet.Append(Convert.ToChar(123).ToString() + "PC020;0270,0250,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //"x(60)" skip.
                   cRet.Append(Convert.ToChar(123).ToString() + "RC20;" + "OUTTER LABEL" + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //"x(60)" skip.
               }
               cRet.Append(Convert.ToChar(123).ToString() + "PC010;0077,0255,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));  //"x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC10;" + t1_ref + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); // "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC011;0600,0135,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC11;" + t1_date_code + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC012;0077,0290,05,05,J,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC12;Cust. : " + t1_cust_part + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC013;0077,0320,05,05,J,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC13;" + t1_mfgr_part + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(80)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC014;0400,0104,05,05,C,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC14;" + "Net Wt(g)" + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC015;0540,0104,05,05,C,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC15;" + t1_wt + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC021;0650,0104,05,05,C,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC021;" + t1_msd + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "PC018;0455,0355,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "RC18;" + t1_sony + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "XS;I," + (cNoLabel).ToString("0000") + ",0002C3200|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));                      //format "x(60)" skip.
               cRet.Append(Convert.ToChar(123).ToString() + "U1;0030|" + Convert.ToChar(125).ToString() + Convert.ToChar(13)); //format "x(60)" skip.
            } else if (t1_printer == "2") {
                cRet.Append(Convert.ToChar(123).ToString() + "D0410,0762,0380|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "C|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "U2;0030|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "AX;+000,+000,+00|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "AY;+10,0|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC019;0077,0017,05,05,E,00,B|"     + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC19;" +  "_____________________________________________|"+ Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC017;0100,0040,05,05,E,00,B|"+ Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC17;" + t1_user + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC016;0300,0040,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC16;" + t1_by + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "XB00;0080,0042,A,3,03,0,0036,+0000000000,000,0,00|"   + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RB00;>7" + t1_RIR + "|"  + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC001;0077,0106,05,05,D,00,B|"  + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC01;" + t1_type + "|"  + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC002;0260,0106,05,05,D,00,B|"  + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC02;" + t1_loc + "|"  + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC003;0500,0074,07,07,D,00,B|"  + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC03;" + t1_RIR + " " + t1_nw_po + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC004;0075,0355,09,09,D,00,B|"  + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC04;" + t1_part + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC006;0077,0140,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC06;" + t1_site + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "XB02;0380,0110,A,3,01,0,0035,+0000000000,000,1,00|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RB02;>7" + t1_qty_per2.Trim() + "|"+ Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC007;0077,0215,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC07;" + t1_qty_tot.Trim() + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC008;0077,0175,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC08;Expiry |" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC009;0174,0175,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC09;" + t1_expi_date.Trim() + " " + '(' + t1_expi_type + ')' + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                if (t1_pims_nbr != "") {
                    cRet.Append(Convert.ToChar(123).ToString() +  "XB03;0270,0180,A,3,03,0,0045,+0000000000,000,1,00|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                    cRet.Append(Convert.ToChar(123).ToString() +  "RB03;>7" + t1_pims_nbr.Trim() + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                }
                else{
                    cRet.Append(Convert.ToChar(123).ToString() +  "PC020;0400,0250,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                    cRet.Append(Convert.ToChar(123).ToString() +  "RC20;" + "OUTTER LABEL"  + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                }
                cRet.Append(Convert.ToChar(123).ToString() + "PC010;0077,0255,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC10;" + t1_ref + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC011;0485,0150,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC11;" + t1_date_code + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC012;0077,0290,05,05,J,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC12;" + t1_cust_part + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC013;0077,0320,05,05,J,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC13;" + t1_mfgr_part + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC014;0350,0106,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC14;" + "Net Wt(g)" + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC015;0550,0106,05,05,D,00,B|"+ Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC15;" + t1_wt.Trim() + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC021;0650,0106,05,05,D,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC21;" + t1_msd + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "PC018;0455,0355,05,05,E,00,B|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "RC18;" + t1_sony + "|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "XS;I," + (cNoLabel).ToString("0000") + ",0002C3200|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
                cRet.Append(Convert.ToChar(123).ToString() + "U1;0030|" + Convert.ToChar(125).ToString() + Convert.ToChar(13));
            } else if (t1_printer == "3") {

            }
            return cRet;
        }
        
    }

}
