package com.example.contactsapp.util;

import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

/**
 * REFERENCE:
 * https://www.mkyong.com/tutorials/hibernate-tutorials/
 */
public class HibernateUtil {

	private static SessionFactory sessionFactory = buildSessionFactory();

	private static SessionFactory buildSessionFactory() {
		try {
			// load from different directory
			SessionFactory sessionFactory = 
				new Configuration()
					.configure("hibernate.cfg.xml")
					.buildSessionFactory();

			return sessionFactory;

		} catch (Throwable ex) {
			// Make sure you log the exception, as it might be swallowed
			System.err.println("Initial SessionFactory creation failed." + ex);
			throw new ExceptionInInitializerError(ex);
		}
	}

	public static SessionFactory getSessionFactory() {
		return sessionFactory;
	}

	public static void shutdown() {
		// Close caches and connection pools
		if (sessionFactory != null)
			sessionFactory.close();
		sessionFactory = null;
	}
	
	public static Session openSession() {
		if (sessionFactory == null)
			sessionFactory = buildSessionFactory();
		return sessionFactory.openSession();		
	}
	
}
