#ifndef CONSUMERLOCKEDQUEUE_H
#define CONSUMERLOCKEDQUEUE_H

#include <boost\lockfree\spsc_queue.hpp>
#include <boost\thread.hpp>
#include <boost\thread\condition.hpp>
#include <boost\thread\xtime.hpp>

//http://www.drdobbs.com/parallel/lock-free-queues/208801974?pgno=2

template <typename T,
	class A0 = boost::parameter::void_,
	class A1 = boost::parameter::void_>
class LockedQueue
{
public:
	bool push(const T& t)
	{
		bool result = m_LockfreeQueue.push(t);
		m_Condition.notify_one();
		return result;
	}

	void pop(T& t)
	{
		boost::mutex::scoped_lock lock(m_Mutex);
		const boost::system_time timeout = boost::get_system_time() + boost::posix_time::milliseconds(4);
		while (!m_LockfreeQueue.pop(t))
		{
			m_Condition.timed_wait(lock, timeout);
		}
	}

	bool empty()
	{
		return m_LockfreeQueue.empty();
	}

private:
	boost::lockfree::spsc_queue<T, A0, A1> m_LockfreeQueue;
	boost::condition m_Condition;
	boost::mutex m_Mutex;
};

#endif

