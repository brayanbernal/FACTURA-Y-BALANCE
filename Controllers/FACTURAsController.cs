using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FACYBAL.Models;
using FACYBAL.Models.ViewModels;

namespace FACYBAL.Controllers
{
	public class FACTURAsController : Controller
	{
		private FACYBALEntities db = new FACYBALEntities();

		// GET: FACTURAs
		public ActionResult reporte(int dia, int mes, int anio)
		{
			//try
			//{
			DateTime fe = Convert.ToDateTime(anio + "-" + mes + "-" + dia + " 00:00:00");
			var fecha1 = fe.ToString("yyyy-MM-dd") + " 00:00:00";
			DateTime fecha = Convert.ToDateTime(fecha1);
			//	DateTime fecha = Convert.ToDateTime(fecha1);


			//para ventas del dia


			System.Nullable<Decimal> VENTA_EFECTIVO = (from ven_efectivo in db.BALANCEs
													   where ven_efectivo.fecha == fecha && ven_efectivo.concepto2 == "VENTA"
													   select ven_efectivo.efectivo).Sum();

			System.Nullable<Decimal> VENTA_TARJETA = (from ven_tarjeta in db.BALANCEs
													  where ven_tarjeta.fecha == fecha && ven_tarjeta.concepto2 == "VENTA"
													  select ven_tarjeta.tarjeta).Sum();

			System.Nullable<Decimal> VENTA_TRANSACCION = (from ven_transferencia in db.BALANCEs
														  where ven_transferencia.fecha == fecha && ven_transferencia.concepto2 == "VENTA"
														  select ven_transferencia.transferencia).Sum();

			System.Nullable<Decimal> VENTA_SISTECREDITO = (from ven_sistecredito in db.BALANCEs
														   where ven_sistecredito.fecha == fecha && ven_sistecredito.concepto2 == "VENTA"
														   select ven_sistecredito.sistecredito).Sum();

			if (VENTA_EFECTIVO == null)
			{
				VENTA_EFECTIVO = 0;
			}

			if (VENTA_TARJETA == null)
			{
				VENTA_TARJETA = 0;
			}

			if (VENTA_TRANSACCION == null)
			{
				VENTA_TRANSACCION = 0;
			}
			if (VENTA_SISTECREDITO == null)
			{
				VENTA_SISTECREDITO = 0;
			}


			// PARA SEPARADOS

			System.Nullable<Decimal> SEP_EFECTIVO = (from ven_efectivo in db.BALANCEs
													 where ven_efectivo.fecha == fecha && ven_efectivo.concepto2 == "SEPARADO"
													 select ven_efectivo.efectivo).Sum();

			System.Nullable<Decimal> SEP_TARJETA = (from ven_tarjeta in db.BALANCEs
													where ven_tarjeta.fecha == fecha && ven_tarjeta.concepto2 == "SEPARADO"
													select ven_tarjeta.tarjeta).Sum();

			System.Nullable<Decimal> SEP_TRANSACCION = (from ven_transferencia in db.BALANCEs
														where ven_transferencia.fecha == fecha && ven_transferencia.concepto2 == "SEPARADO"
														select ven_transferencia.transferencia).Sum();

			System.Nullable<Decimal> SEP_SISTECREDITO = (from ven_sistecredito in db.BALANCEs
														 where ven_sistecredito.fecha == fecha && ven_sistecredito.concepto2 == "SEPARADO"
														 select ven_sistecredito.sistecredito).Sum();

			if (SEP_EFECTIVO == null)
			{
				SEP_EFECTIVO = 0;
			}

			if (SEP_TARJETA == null)
			{
				SEP_TARJETA = 0;
			}

			if (SEP_TRANSACCION == null)
			{
				SEP_TRANSACCION = 0;
			}
			if (SEP_SISTECREDITO == null)
			{
				SEP_SISTECREDITO = 0;
			}

					   			 		  		  		 	   			
			ViewBag.ven_efe = Convert.ToDecimal(VENTA_EFECTIVO);
			ViewBag.ven_tar = Convert.ToDecimal(VENTA_TARJETA);
			ViewBag.ven_trans = Convert.ToDecimal(VENTA_TRANSACCION);
			ViewBag.ven_sist = Convert.ToDecimal(VENTA_SISTECREDITO);

			ViewBag.sep_efe = Convert.ToDecimal(SEP_EFECTIVO);
			ViewBag.sep_tar = Convert.ToDecimal(SEP_TARJETA);
			ViewBag.sep_trans = Convert.ToDecimal(SEP_TRANSACCION);

			ViewBag.tot_efe = Convert.ToDecimal(VENTA_EFECTIVO) + Convert.ToDecimal(SEP_EFECTIVO);
			ViewBag.tot_tar = Convert.ToDecimal(VENTA_TARJETA) + Convert.ToDecimal(SEP_TARJETA);
			ViewBag.tot_tra = Convert.ToDecimal(VENTA_TRANSACCION) + Convert.ToDecimal(SEP_TRANSACCION);
			ViewBag.tot_sist = Convert.ToDecimal(VENTA_SISTECREDITO);
			ViewBag.total = Convert.ToDecimal(VENTA_EFECTIVO + VENTA_TARJETA + VENTA_TRANSACCION + VENTA_SISTECREDITO + SEP_EFECTIVO + SEP_TARJETA + SEP_TRANSACCION);

			ViewBag.fech = dia + "/" + mes + "/" + anio;

			IEnumerable<FACYBAL.Models.FACTURA> facturas = db.FACTURAs.Where(i => i.fecha == fecha).ToList();
			IEnumerable<FACYBAL.Models.ITEM_VENTA> itemVentas = db.ITEM_VENTA.ToList();

			// Crea una instancia del modelo de vista y asigna los datos
			FacturaItemViewModel viewModel = new FacturaItemViewModel
			{
				Facturas = facturas,
				ItemVentas = itemVentas
			};

			return View(viewModel);
		}

		public ActionResult Index()
		{
			var fACTURAs = db.FACTURAs.Include(f => f.EMPLEADO);
			return View(fACTURAs.ToList());
		}
		public ActionResult IndexAbonos()
		{
			var fACTURAs = db.FACTURAs.Where(i => i.sep_vent == "SEPARADO" || i.sistecredito>0).Include(f => f.EMPLEADO);
			return View(fACTURAs.ToList());
		}

		// GET: FACTURAs/Details/5
		public ActionResult DetailsVent(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTURA fACTURA = db.FACTURAs.Find(id);
			if (fACTURA == null)
			{
				return HttpNotFound();
			}
			return View(fACTURA);
		}
		public ActionResult DetailsSep(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTURA fACTURA = db.FACTURAs.Find(id);
			decimal saldo = Convert.ToDecimal(fACTURA.total_venta) - Convert.ToDecimal(fACTURA.efectivo) - Convert.ToDecimal(fACTURA.tarjeta) - Convert.ToDecimal(fACTURA.transferencia);
			ViewBag.saldo = saldo;
			if (fACTURA == null)
			{
				return HttpNotFound();
			}
			return View(fACTURA);
		}

		// GET: FACTURAs/Create
		
		public ActionResult Create()
		{
			ViewBag.n_empl = new SelectList(db.EMPLEADOes, "id", "n_empleado");
			return View();
		}


		// POST: FACTURAs/Create
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(VentasViewmodel model)
		{
			try
			{
				using (FACYBALEntities db = new FACYBALEntities())
				{
					decimal costoTotal = 0;
					decimal totalVenta = 0;

					FACTURA fACTURA = new FACTURA();
					fACTURA.fecha = DateTime.Now;
					fACTURA.efectivo = model.efectivo;
					fACTURA.tarjeta = model.tarjeta;
					fACTURA.transferencia = model.transferencia;
					fACTURA.sistecredito = model.sistecredito;
					fACTURA.tipo_tarjeta = model.tipo_tarjeta;
					fACTURA.t_cred_deb = model.t_cred_deb;
					fACTURA.sep_vent = model.sep_vent;
					fACTURA.n_cliente = model.n_cliente.ToUpper();
					fACTURA.doc_cliente = model.doc_cliente;
					fACTURA.n_empl = model.n_empl;
					fACTURA.tel = model.tel;
					fACTURA.total_venta = 0;
					fACTURA.fecha_sep = DateTime.Now;
					fACTURA.costo_tot = 0;
					CONSECUTIVO cons = db.CONSECUTIVOes.Find(1);
					fACTURA.consecutivo = cons.con_act;
					db.FACTURAs.Add(fACTURA);
					db.SaveChanges();
					cons.con_act = cons.con_act + 1;
					db.SaveChanges();

					foreach (var item in model.items)
					{
						ITEM_VENTA it = new ITEM_VENTA();
						it.id_venta = fACTURA.id;
						it.codigo1 = item.codigo1;
						it.codigo2 = item.codigo2;
						it.descripcion = item.descripcion.ToUpper();
						it.precio = item.precio;
						it.cantidad = item.cantidad;
						it.tot_articulo = item.precio * item.cantidad;
						costoTotal = (Convert.ToDecimal(it.codigo2 * 1000) * Convert.ToDecimal(it.cantidad)) + costoTotal;
						totalVenta = Convert.ToDecimal(it.tot_articulo) + totalVenta;
						db.ITEM_VENTA.Add(it);
					}
					db.SaveChanges();
					fACTURA.total_venta = totalVenta;
					fACTURA.costo_tot = costoTotal;
					db.SaveChanges();


					//////////////////////// agregamos datos en balance ///////////////
					BALANCE bal = new BALANCE();
					bal.fecha = fACTURA.fecha;
					bal.cons_factura = fACTURA.consecutivo;
					bal.id_factura = fACTURA.id;
					bal.concepto1 = fACTURA.doc_cliente + "||" + fACTURA.n_cliente;
					bal.concepto2 = fACTURA.sep_vent;
					bal.concepto3 = fACTURA.t_cred_deb+" "+fACTURA.tipo_tarjeta ;
					bal.costo_tot = fACTURA.costo_tot;
					bal.efectivo = fACTURA.efectivo;				
					bal.tarjeta = fACTURA.tarjeta;				
					bal.transferencia = fACTURA.transferencia;				
					bal.sistecredito = fACTURA.sistecredito;					
					decimal tot_ingreso = Convert.ToDecimal(fACTURA.efectivo + fACTURA.tarjeta + fACTURA.transferencia);
					bal.ingreso = tot_ingreso;
					bal.egreso = 0;
					db.BALANCEs.Add(bal);
					db.SaveChanges();

					////////////////////////// fin de datos en balance/////////////////////////////////////////

					//ViewBag.n_empl = new SelectList(db.EMPLEADOes, "id", "n_empleado");
					if (model.sep_vent == "VENTA")
					{
						return RedirectToAction("DetailsVent", new { id = fACTURA.id });
					}
					else
					{
						return RedirectToAction("DetailsSep", new { id = fACTURA.id });
					}

				}

			}
			catch (Exception ex)
			{
				ViewBag.n_empl = new SelectList(db.EMPLEADOes, "id", "n_empleado");
				return View();

			}
		}

		// GET: FACTURAs/Edit/5
		public ActionResult Abono(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTURA fACTURA = db.FACTURAs.Find(id);
			if (fACTURA == null)
			{
				return HttpNotFound();
			}
			ViewBag.n_empl = new SelectList(db.EMPLEADOes, "id", "n_empleado", fACTURA.n_empl);
			return View(fACTURA);
		}

		// POST: FACTURAs/Edit/5
		// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
		// más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Abono([Bind(Include = "id,fecha,efectivo,tarjeta,tipo_tarjeta,t_cred_deb,sep_vent,total_venta,consecutivo,n_cliente,doc_cliente,n_empl,tel,costo_tot,fecha_sep,transferencia,sistecredito")] FACTURA fACTURA, decimal efectivo1,decimal tarjeta1, decimal transferencia1)
		{
			if (ModelState.IsValid)
			{
				fACTURA.efectivo = fACTURA.efectivo + efectivo1;
				fACTURA.tarjeta = fACTURA.tarjeta + tarjeta1;
				fACTURA.transferencia = fACTURA.transferencia + transferencia1;

				db.Entry(fACTURA).State = EntityState.Modified;
				db.SaveChanges();

				BALANCE bal = new BALANCE();
				bal.fecha = fACTURA.fecha;
				bal.cons_factura = fACTURA.consecutivo;
				bal.id_factura = fACTURA.id;
				bal.concepto1 = fACTURA.doc_cliente + "||" + fACTURA.n_cliente;
				bal.concepto2 = fACTURA.sep_vent;
				bal.concepto3 = fACTURA.t_cred_deb + " " + fACTURA.tipo_tarjeta;
				bal.costo_tot = fACTURA.costo_tot;
				bal.efectivo = efectivo1;
				bal.tarjeta = tarjeta1;
				bal.transferencia = transferencia1;
				bal.sistecredito = fACTURA.sistecredito;
				decimal tot_ingreso = Convert.ToDecimal(fACTURA.efectivo + fACTURA.tarjeta + fACTURA.transferencia);
				bal.ingreso = tot_ingreso;
				bal.egreso = 0;
				db.BALANCEs.Add(bal);
				db.SaveChanges();
				return RedirectToAction("DetailsSep", new { id = fACTURA.id });
			}
			ViewBag.n_empl = new SelectList(db.EMPLEADOes, "id", "n_empleado", fACTURA.n_empl);
			return View(fACTURA);
		}

		// GET: FACTURAs/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			FACTURA fACTURA = db.FACTURAs.Find(id);
			if (fACTURA == null)
			{
				return HttpNotFound();
			}
			return View(fACTURA);
		}

		// POST: FACTURAs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			FACTURA fACTURA = db.FACTURAs.Find(id);
			db.FACTURAs.Remove(fACTURA);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
